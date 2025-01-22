using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Repositorios.Interfaces;


namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/generos")]
public class GenerosController : ControllerBase
{
    private readonly IRepositorioEnMemoria _repositorio;
    private readonly IOutputCacheStore _cacheStore;
    private const string CacheKey = "Generos";

    public GenerosController(IRepositorioEnMemoria repositorio, IOutputCacheStore cacheStore)
    {
        _repositorio = repositorio;
        _cacheStore = cacheStore;
    }
    
    [HttpGet]
    [OutputCache(Tags = [CacheKey])]
    public IActionResult GetGeneros()
    {
        List<Genero> generos = _repositorio.ObtenerTodosLosGeneros();
        
        return Ok(generos);
    }

    [HttpGet("{id}")]
    [OutputCache(Tags = [CacheKey])]
    public async Task<ActionResult<Genero>> GetGenero(Guid id)
    {
        Genero? genero = await _repositorio.ObtenerGeneroPorId((Guid) id);
        
        if (genero == null)
        {
            return NotFound("Genero no encontrado");
        }
        
        return Ok(genero);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Genero genero)
    {
        bool existe = _repositorio.Existe(genero.Nombre);

        if (existe)
        {
            return Conflict("Genero ya existe");
        }
        
        _repositorio.AgregarGenero(genero);
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        return Created("", genero);
    }
}