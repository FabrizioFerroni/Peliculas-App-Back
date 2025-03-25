using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Annotations;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Dtos.Response;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Servicios.Interfaces;
using PeliculasApp_Back.Utilidades;

namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/actores")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esadmin")]
public class ActoresController : CustomBaseController
{
    private readonly IOutputCacheStore _cacheStore;
    private const string CacheKey = "Actores";
    private readonly string Contenedor = "actores";
    private readonly ApplicationDbContext _context; //borrar luego
    private readonly IMapper _mapper;
    private readonly IAlmacenadorArchivos _almacenadorArchivos;

    public ActoresController(IOutputCacheStore cacheStore, ApplicationDbContext context, IMapper mapper,
        IAlmacenadorArchivos almacenadorArchivos) : base(context, mapper)
    {
        _cacheStore = cacheStore;
        _context = context;
        _mapper = mapper;
        _almacenadorArchivos = almacenadorArchivos;
    }

    [HttpGet]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Pageable<List<ActorDto>>>> GetActores([FromQuery] PaginationDto pagination)
    {
        Pageable<List<ActorDto>> response = await Get<Actor, ActorDto>(pagination, buscarPor: a => a.Nombre);

        return Ok(response);
    }

    [HttpGet("{id}")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ActorDto>> GetActor(Guid id)
    {
        ActionResult<ActorDto> actor = await GetById<Actor, ActorDto>(id, "Actor no encontrado");

        return actor;
    }

    [HttpGet("n/{nombre}")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<PeliculaActorDto>>> GetActorByNombre(string nombre)
    {
        List<PeliculaActorDto> actores = await _context.Actores.ProjectTo<PeliculaActorDto>(_mapper.ConfigurationProvider).Where(e => e.Nombre.Contains(nombre)).ToListAsync();
        
        return Ok(actores);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CrearActor([FromForm] ActorCreacionDto dto)
    {
        //TODO: Antes de todo tendria que comprobar que lo que viene en nombre no exista en la bd para que no de error o controlar el error que lanza sql.
        
        //bool existe = await ExisteGenero(dto.Nombre);
        bool existe = await _context.Actores.AnyAsync(g => g.Nombre == dto.Nombre);

        if (existe)
            return BadRequest(new { Mensaje = "El actor con ese Nombre ya existe" });
        
        Actor actor = _mapper.Map<Actor>(dto);

        if (dto.Foto != null)
        {
            string url = await _almacenadorArchivos.Almacenar(Contenedor, dto.Foto);
            actor.Foto = url;
        }
        
        actor.Slug = GenerateSlug.URLFriendly(dto.Nombre);
        
        await _context.AddAsync(actor);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        ActorDto actorDto = _mapper.Map<ActorDto>(actor);
        
        return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actorDto);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Put(Guid id, [FromForm] ActorCreacionDto dto)
    {
        Actor? actor = await _context.Actores.FindAsync(id);
        
        if (actor is null)
            return NotFound(new { Mensaje = "Actor no encontrado" });
        
        bool existe = await _context.Actores.AnyAsync(g => g.Nombre == dto.Nombre && g.Id != id);

        if (existe)
            return BadRequest(new { Mensaje = "El actor con ese Nombre ya existe" });
        
        _mapper.Map(dto, actor);
        
        if (dto.Foto is not null)
        {
            actor.Foto = await _almacenadorArchivos.Editar(actor.Foto, Contenedor, dto.Foto);
        }
        
        actor.Id = id;
        actor.FechaModificacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Delete(Guid id)
    {
        Actor? actor = await _context.Actores.FindAsync(id);
        
        if(actor == null)
            return NotFound(new { Mensaje = "Actor no encontrado" });

        if (actor.Foto is not null)
        {
            await _almacenadorArchivos.Borrar(actor.Foto, Contenedor);
        }
        
        _context.Actores.Remove(actor);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        return NoContent();
    }
}