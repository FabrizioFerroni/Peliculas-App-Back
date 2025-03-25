using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Annotations;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Dtos.Response;
using PeliculasApp_Back.Utilidades;

namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esadmin")]
public class UsuariosController: ControllerBase
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UsuariosController(UserManager<IdentityUser<Guid>> userManager, SignInManager<IdentityUser<Guid>> signInManager, IConfiguration config, ApplicationDbContext context, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _context = context;
        _mapper = mapper;
    }

   [HttpGet("")]
    public async Task<ActionResult<Pageable<List<UsuarioDTO>>>> ListadoUsuarios([FromQuery] PaginationDto pagination)
    {
        IQueryable<IdentityUser<Guid>> queryable = _context.Users.AsQueryable();
        
        int totalElements = await queryable.CountAsync();
        
        queryable = pagination.Ascending
            ? queryable.OrderBy(e => EF.Property<object>(e, pagination.Ordenar))
            : queryable.OrderByDescending(e => EF.Property<object>(e, pagination.Ordenar));
        
        List<UsuarioDTO> resDto = await queryable
            .Paginar(pagination)
            .ProjectTo<UsuarioDTO>(_mapper.ConfigurationProvider).ToListAsync();

        Pageable<List<UsuarioDTO>> response = PageableResponse.CreatePageableResponse(resDto, pagination.Pagina, pagination.RegistrosPorPagina, totalElements);

        return response;
    }
    
    [HttpPost("add-rol")]
    [AllowAnonymous]
    public async Task<IActionResult> HacerAdmin(EditarClaimDto dto)
    {
        IdentityUser<Guid>? user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }


        await _userManager.AddClaimAsync(user, new Claim("esadmin", "true"));
        return NoContent();
    }
    
    [HttpPost("delete-rol")]
    [AllowAnonymous]
    public async Task<IActionResult> QuitarAdmin(EditarClaimDto dto)
    {
        IdentityUser<Guid>? user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }


        await _userManager.RemoveClaimAsync(user, new Claim("esadmin", "true"));
        return NoContent();
    }
}