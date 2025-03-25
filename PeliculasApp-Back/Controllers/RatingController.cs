using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Servicios.Interfaces;

namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/rating")]
public class RatingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IServicioUsuarios _userService;

    public RatingController(ApplicationDbContext context, IServicioUsuarios userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post([FromBody] RatingCreacionDto dto)
    {
        Guid userID = await _userService.ObtenerUsuario();
        
        Rating? ratingActual = await _context.RatingsPeliculas.FirstOrDefaultAsync(r => r.PeliculaId == dto.PeliculaId && r.UsuarioId == userID);

        if (ratingActual is null)
        {
            Rating rating = new Rating()
            {
                PeliculaId = dto.PeliculaId,
                Puntuacion = dto.Puntuacion,
                UsuarioId = userID,
                FechaCreacion = DateTime.Now
            };
            
            _context.Add(rating);
        }
        else
        {
            ratingActual.Puntuacion = dto.Puntuacion;
            ratingActual.FechaModificacion = DateTime.Now;
        }
        
        await _context.SaveChangesAsync();
        return Ok();
    }

}