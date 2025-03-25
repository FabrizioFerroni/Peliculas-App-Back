using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PeliculasApp_Back.Dtos;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace PeliculasApp_Back.Controllers;

[ApiController]
[Route("auth")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esadmin")]
public class AuthController: ControllerBase
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<IdentityUser<Guid>> userManager, SignInManager<IdentityUser<Guid>> signInManager, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RespuestaAuthDto>> Register(RegisterDto dto)
    {
        IdentityUser<Guid>? user = new IdentityUser<Guid>
        {
            UserName = dto.UserName,
            Email = dto.Email,
            
        };
        
        IdentityResult? result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return await CreateToken(user);
        
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<RespuestaAuthDto>> Login(LoginDto dto)
    {
        IdentityUser<Guid>? user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null)
        {
            IEnumerable<IdentityError> error = ConstruirLoginInvalidos();
            return BadRequest(error);
        }
        
        // var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, true);
        SignInResult? result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            IEnumerable<IdentityError> error = ConstruirLoginInvalidos();
            return BadRequest(error);
        }
        
        return await CreateToken(user);
    }

    private IEnumerable<IdentityError> ConstruirLoginInvalidos()
    {
        IdentityError? identityErrors = new IdentityError(){Description = "Login incorrecto" };
        List<IdentityError>? errors = new List<IdentityError>();
        errors.Add(identityErrors);
        return errors;
    }

    private async Task<RespuestaAuthDto> CreateToken(IdentityUser<Guid> user)
    {
        List<Claim>? claims = new()
        {
            new Claim("email", user.Email!),
            new Claim("username", user.UserName!)
        };
        
        IList<Claim>? claimsDB = await _userManager.GetClaimsAsync(user);
        
        claims.AddRange(claimsDB);

        SymmetricSecurityKey? llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]!));
        SigningCredentials? creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        
        DateTime expires = DateTime.Now.AddHours(1);

        JwtSecurityToken? tokenSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expires, signingCredentials: creds);
        
        string? token = new JwtSecurityTokenHandler().WriteToken(tokenSeguridad);


        return new RespuestaAuthDto()
        {
            Token = token,
            TokenExpiry = expires
        };
    }
}