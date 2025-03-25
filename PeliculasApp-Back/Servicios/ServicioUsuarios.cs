using Microsoft.AspNetCore.Identity;
using PeliculasApp_Back.Servicios.Interfaces;

namespace PeliculasApp_Back.Servicios;

public class ServicioUsuarios : IServicioUsuarios
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser<Guid>> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<Guid> ObtenerUsuario()
    {
        string email = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "email")!.Value;
        IdentityUser<Guid>? user = await _userManager.FindByEmailAsync(email);
        return user!.Id;
    }
}