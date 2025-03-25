using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeliculasApp_Back.Annotations;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Dtos.Response;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Servicios.Interfaces;
using PeliculasApp_Back.Utilidades;

namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/peliculas")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esadmin")]
public class PeliculasController : CustomBaseController
{
    private readonly IOutputCacheStore _cacheStore;
    private const string CacheKey = "Peliculas";
    private readonly string Contenedor = "peliculas";
    private readonly ApplicationDbContext _context; //borrar luego
    private readonly IMapper _mapper;
    private readonly IAlmacenadorArchivos _almacenadorArchivos;
    private readonly IServicioUsuarios _userService;

    public PeliculasController(IOutputCacheStore cacheStore, ApplicationDbContext context, IMapper mapper,
        IAlmacenadorArchivos almacenadorArchivos, IServicioUsuarios userService) : base(context, mapper)
    {
        _cacheStore = cacheStore;
        _context = context;
        _mapper = mapper;
        _almacenadorArchivos = almacenadorArchivos;
        _userService = userService;
    }

    [HttpGet("nuevo/cine-generos")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PeliculasPostGetDto>> GetFiltrar()
    {
        List<CineDto> cines = await _context.Cines.ProjectTo<CineDto>(_mapper.ConfigurationProvider).ToListAsync();
        List<GeneroDto> generos =
            await _context.Generos.ProjectTo<GeneroDto>(_mapper.ConfigurationProvider).ToListAsync();

        PeliculasPostGetDto peliculas = new PeliculasPostGetDto()
        {
            Cines = cines,
            Generos = generos,
        };

        return Ok(peliculas);
    }

    [HttpGet("filtrar")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [AllowAnonymous]
    public async Task<ActionResult<Pageable<List<PeliculaDto>>>> Filtrar([FromQuery] PeliculaFiltrarDto dto)
    {
        IQueryable<Pelicula>? peliculasQueryable = _context.Peliculas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Titulo))
        {
            peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.Contains(dto.Titulo));
        }

        if (dto.EnCines)
        {
            peliculasQueryable = peliculasQueryable.Where(p => p.PeliculasCines.Select(pc => pc.PeliculaId).Contains(p.Id));
        }

        if (dto.ProximoEstreno)
        {
            DateTime hoy = DateTime.Today;
            peliculasQueryable = peliculasQueryable.Where(p => p.FechaCreacion > hoy);
        }
        
        if (dto.Genero is not null)
        {
            Genero? genero = await _context.Generos.OrderBy(g => g.Nombre).FirstOrDefaultAsync(g => g.Slug.ToLower() == dto.Genero.Trim().ToLower());

            if (genero is null)
            {
                return NotFound("El slug por el cual intentas buscar no existe");
            }
            
            peliculasQueryable = peliculasQueryable.
                Where(p => p.PeliculasGeneros.Select(pg => pg.GeneroId).Contains(genero.Id));
        }
        
        if (!string.IsNullOrWhiteSpace(dto.AnioLanzamiento))
        {
            peliculasQueryable = peliculasQueryable.Where(p => p.AnioLanzamiento.Contains(dto.AnioLanzamiento));
        }
        
        int totalElements = await peliculasQueryable.CountAsync();
        
        List<PeliculaDto> resDto = await peliculasQueryable
            .Paginar(dto.Paginacion)
            .OrderBy(p => p.Titulo)
            .ProjectTo<PeliculaDto>(_mapper.ConfigurationProvider).ToListAsync();

        Pageable<List<PeliculaDto>> response = PageableResponse.CreatePageableResponse(resDto, dto.Pagina, dto.RecordsPorPagina, totalElements);

        return response;
    }

    [HttpGet("landing")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [AllowAnonymous]
    public async Task<ActionResult<LandingPageDto>> GetLanding()
    {
        int top = 6;
        DateTime today = DateTime.Now;

        List<PeliculaDto> proximosEstrenos = await _context.Peliculas
            .Where(p => p.FechaLanzamiento > today)
            .OrderBy(p => p.FechaLanzamiento)
            .Take(top)
            .ProjectTo<PeliculaDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        List<PeliculaDto> enCines = await _context.Peliculas
            .Where(p => p.PeliculasCines.Select(pc => pc.PeliculaId).Contains(p.Id))
            .OrderBy(p => p.FechaLanzamiento)
            .Take(top)
            .ProjectTo<PeliculaDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        LandingPageDto response = new LandingPageDto();
        response.EnCines = enCines;
        response.ProximosEstrenos = proximosEstrenos;
        
        return Ok(response);
    }
    
    [HttpGet("{slug}")]
    [OutputCache(Tags = [CacheKey])]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [AllowAnonymous]
    public async Task<ActionResult<PeliculaDetallesDto>> GetPeliculaBySlug(string slug)
    {
        PeliculaDetallesDto? pelicula = await _context.Peliculas.ProjectTo<PeliculaDetallesDto>(_mapper.ConfigurationProvider).Where(e => e.Slug!.Contains(slug)).FirstOrDefaultAsync();

        if (pelicula is null)
        {
            return NotFound(new { Mensaje = "Pelicula no encontrada con ese slug"});
        }

        double promedioVoto = 0.0;
        int usuarioVoto = 0;

        if (await _context.RatingsPeliculas.AnyAsync(r => r.PeliculaId == pelicula.Id))
        {
            promedioVoto = await _context.RatingsPeliculas.Where(r => r.PeliculaId == pelicula.Id).AverageAsync(r => r.Puntuacion);
            
            if (HttpContext.User.Identity!.IsAuthenticated)
            {
                Guid userId = await _userService.ObtenerUsuario();
                
                Rating? ratingDB = await _context.RatingsPeliculas.FirstOrDefaultAsync(r =>  r.UsuarioId == userId && r.PeliculaId == pelicula.Id);

                if (ratingDB is not null)
                {
                    usuarioVoto = ratingDB.Puntuacion;
                }
            }
        }

        pelicula.PromedioVoto = promedioVoto;
        pelicula.VotoUsuario = usuarioVoto;
        
        return Ok(pelicula);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CrearPelicula([FromForm] PeliculaCreacionDto dto)
    {
        //TODO: Antes de todo tendria que comprobar que lo que viene en nombre no exista en la bd para que no de error o controlar el error que lanza sql.
        
        bool existe = await _context.Actores.AnyAsync(g => g.Nombre == dto.Titulo);

        if (existe)
            return BadRequest(new { Mensaje = "La pelicula con ese Titulo ya existe" });
        
        Pelicula pelicula = _mapper.Map<Pelicula>(dto);

        if (dto.Poster is not null)
        {
            string url = await _almacenadorArchivos.Almacenar(Contenedor, dto.Poster);
            pelicula.Poster = url;
        }
        
        AsignarOrdenActores(pelicula);
        
        pelicula.Slug = GenerateSlug.URLFriendly(dto.Titulo);
        
        await _context.AddAsync(pelicula);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        PeliculaDto peliculaDto = _mapper.Map<PeliculaDto>(pelicula);
        
        return CreatedAtAction(nameof(GetPeliculaBySlug), new { slug = pelicula.Slug }, peliculaDto);
    }

    
    [HttpGet("p/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PeliculaPutDto>> GetPelicula(Guid id)
    {
        PeliculaDetallesDto? pelicula = await _context.Peliculas
            .ProjectTo<PeliculaDetallesDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(p => p.Id.Equals(id));
        
        if (pelicula is null)
        {
            return NotFound(new { Mensaje = "Pelicula no encontrada con ese id"});
        }
        
        List<Guid> generosSeleccionadosIds = pelicula.Generos.Select(g => g.Id).ToList();
        List<GeneroDto> generosNoSeleccionados = await _context.Generos.Where(g=> !generosSeleccionadosIds.Contains(g.Id)).ProjectTo<GeneroDto>(_mapper.ConfigurationProvider).ToListAsync();
        
        List<Guid> cinesSeleccionadosIds = pelicula.Cines.Select(c => c.Id).ToList();
        List<CineDto> cinesNoSeleccionados =
            await _context.Cines.Where(c => !cinesSeleccionadosIds.Contains(c.Id)).ProjectTo<CineDto>(_mapper.ConfigurationProvider).ToListAsync();

        PeliculaPutDto respuesta = new PeliculaPutDto();

        respuesta.Pelicula = pelicula;
        respuesta.GenerosSeleccionados = pelicula.Generos;
        respuesta.GenerosNoSeleccionados = generosNoSeleccionados;
        respuesta.CineSeleccionados = pelicula.Cines;
        respuesta.CineNoSeleccionados = cinesNoSeleccionados;
        respuesta.Actores = pelicula.Actores;
        
        return Ok(respuesta);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> EditarPelicula(Guid id, [FromForm] PeliculaCreacionDto dto)
    {
        var pelicula = await _context.Peliculas
            .Include(p => p.PeliculasActores)
            .Include(p => p.PeliculasCines)
            .Include(p => p.PeliculasGeneros)
            .FirstOrDefaultAsync(p => p.Id.Equals(id));
        
        if (pelicula is null)
        {
            return NotFound(new { Mensaje = "Pelicula no encontrada con ese id"});
        }
        
        
        pelicula = _mapper.Map(dto, pelicula);
        
        if (dto.Poster is not null)
        {
            pelicula.Poster = await _almacenadorArchivos.Editar(pelicula.Poster, Contenedor, dto.Poster);
        }
        
        AsignarOrdenActores(pelicula);
        //TODO: Ver de validar el slug para que no se repita.
        pelicula.Slug = dto.Titulo != pelicula.Titulo ? GenerateSlug.URLFriendly(dto.Titulo) : pelicula.Slug;
        pelicula.Id = id;
        pelicula.FechaModificacion = DateTime.Now;
        
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
        Pelicula? pelicula = await _context.Peliculas.FindAsync(id);
        
        if(pelicula is null)
            return NotFound(new { Mensaje = "Pelicula no encontrada" });

        if (pelicula.Poster is not null)
        {
            await _almacenadorArchivos.Borrar(pelicula.Poster, Contenedor);
        }
        
        _context.Peliculas.Remove(pelicula);
        await _context.SaveChangesAsync();
        
        await _cacheStore.EvictByTagAsync(CacheKey, default);
        
        return NoContent();
    }


    private void AsignarOrdenActores(Pelicula pelicula)
    {
        if (pelicula.PeliculasActores is not null)
        {
            for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
            {
                pelicula.PeliculasActores[i].Orden = i;
            }
        }
    }
}