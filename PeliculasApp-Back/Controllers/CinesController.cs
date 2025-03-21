using AutoMapper;
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
[Route("api/cines")]
public class CinesController : CustomBaseController
{
    private readonly IOutputCacheStore _cacheStore;
    private const string CacheKey = "Cines";
    private readonly ApplicationDbContext _context; //borrar luego
    private readonly IMapper _mapper;
    
    public CinesController(IOutputCacheStore cacheStore, ApplicationDbContext context, IMapper mapper): base(context, mapper)
    {
        _cacheStore = cacheStore;
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Pageable<List<CineDto>>>> GetCines([FromQuery] PaginationDto pagination)
    {
        Pageable<List<CineDto>> response = await Get<Cine, CineDto>(pagination, buscarPor: c => c.Nombre);

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CineDto>> GetCine(Guid id)
    {
        ActionResult<CineDto> cine = await GetById<Cine, CineDto>(id, "Cine no encontrado");
            
        return cine;
    }
    
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Post([FromBody] CineCreacionDto dto)
    {
        
        //TODO: Antes de todo tendria que comprobar que lo que viene en nombre no exista en la bd para que no de error o controlar el error que lanza sql.
        
        bool existe = await _context.Cines.AnyAsync(g => g.Nombre == dto.Nombre);

        if (existe)
            return BadRequest(new { Mensaje = "El cine con ese Nombre ya existe" });
        
        Cine cine = _mapper.Map<Cine>(dto);
        
        cine.Slug = GenerateSlug.URLFriendly(dto.Nombre);
        
        await _context.AddAsync(cine);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        CineDto cineDto = _mapper.Map<CineDto>(cine);
        
        return CreatedAtAction(nameof(GetCine), new { id = cine.Id }, cineDto);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Put(Guid id, [FromBody] CineCreacionDto dto)
    {
        Cine? cine = await _context.Cines.FindAsync(id);
        
        if (cine == null)
            return NotFound(new { Mensaje = "Cine no encontrado" });
        
        bool existe = await _context.Cines.AnyAsync(c => c.Nombre == dto.Nombre && c.Id != id);

        if (existe)
            return BadRequest(new { Mensaje = "El cine con ese Nombre ya existe" });
        
        _mapper.Map(dto, cine);
        
        cine.Id = id;
        cine.Slug = GenerateSlug.URLFriendly(dto.Nombre);
        cine.FechaModificacion = DateTime.Now;
        
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
        Cine? cine = await _context.Cines.FindAsync(id);
        
        if(cine == null)
            return NotFound(new { Mensaje = "Cine no encontrado" });
        
        _context.Cines.Remove(cine);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        return NoContent();
    }
}