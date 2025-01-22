using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Repositorio;

namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/generos")]
public class GenerosController : ControllerBase
{
    
    
    [HttpGet]
    public IActionResult GetGeneros()
    {
        RepositorioEnMemoria repository = new RepositorioEnMemoria();
        List<Genero> generos = repository.ObtenerTodosLosGeneros();
        
        return Ok(generos);
    }

    [HttpGet("{id}")]
    [OutputCache]
    public async Task<ActionResult<Genero>> GetGenero(Guid id)
    {
        RepositorioEnMemoria repository = new RepositorioEnMemoria();
        Genero? genero = await repository.ObtenerGeneroPorId((Guid) id);
        
        if (genero == null)
        {
            return NotFound("Genero no encontrado");
        }
        
        return Ok(genero);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Genero genero)
    {
        RepositorioEnMemoria repository = new RepositorioEnMemoria();
        repository.AgregarGenero(genero);
        
        return Created("", genero);
    }
}