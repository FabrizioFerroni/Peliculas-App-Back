using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Annotations;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Utilidades;


namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/generos")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esadmin")]
public class GenerosController : CustomBaseController
{
    private readonly IOutputCacheStore _cacheStore;
    private const string CacheKey = "Generos";
    private readonly ApplicationDbContext _context; //borrar luego
    private readonly IMapper _mapper;

    public GenerosController(IOutputCacheStore cacheStore, ApplicationDbContext context, IMapper mapper): base(context, mapper)
    {
        _cacheStore = cacheStore;
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Pageable<List<GeneroDto>>>> GetGeneros([FromQuery] PaginationDto pagination)
    {
        Pageable<List<GeneroDto>> response = await Get<Genero, GeneroDto>(pagination, buscarPor: g => g.Nombre);

        return Ok(response);
    }
    
    [HttpGet("todos")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    [AllowAnonymous]
    public async Task<ActionResult<List<GeneroDto>>> GetAllGeneros()
    {
        List<GeneroDto> response = await GetAllAsync<Genero, GeneroDto>(ordenarPor: g => g.Nombre);

        return Ok(response);
    }

    [HttpGet("{id}")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<GeneroDto>> GetGenero(Guid id)
    {
        ActionResult<GeneroDto> genero = await GetById<Genero, GeneroDto>(id, "Genero no encontrado");
            
        return genero;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Post([FromBody] GeneroCreacionDto dto)
    {
        
        //TODO: Antes de todo tendria que comprobar que lo que viene en nombre no exista en la bd para que no de error o controlar el error que lanza sql.
        
        //bool existe = await ExisteGenero(dto.Nombre);
        bool existe = await _context.Generos.AnyAsync(g => g.Nombre == dto.Nombre);

        if (existe)
            return BadRequest(new { Mensaje = "El genero con ese Nombre ya existe" });
        
        Genero genero = _mapper.Map<Genero>(dto);
        
        genero.Slug = GenerateSlug.URLFriendly(dto.Nombre);
        
        await _context.AddAsync(genero);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        GeneroDto generoDto = _mapper.Map<GeneroDto>(genero);
        
        return CreatedAtAction(nameof(GetGenero), new { id = genero.Id }, generoDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Put(Guid id, [FromBody] GeneroCreacionDto dto)
    {
        Genero? genero = await _context.Generos.FindAsync(id);
        
        if (genero == null)
            return NotFound(new { Mensaje = "Genero no encontrado" });
        
        bool existe = await _context.Generos.AnyAsync(g => g.Nombre == dto.Nombre && g.Id != id);

        if (existe)
            return BadRequest(new { Mensaje = "El genero con ese Nombre ya existe" });
        
        _mapper.Map(dto, genero);
        
        genero.Id = id;
        genero.FechaModificacion = DateTime.Now;
        
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
        Genero? genero = await _context.Generos.FindAsync(id);
        
        if(genero == null)
            return NotFound(new { Mensaje = "Genero no encontrado" });
        
        _context.Generos.Remove(genero);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        return NoContent();
    }
}