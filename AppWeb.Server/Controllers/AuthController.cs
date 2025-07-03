using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using AppWeb.Application.Graphql.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppWeb.Shared.Dtos;
using MediatR;

namespace AppWeb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    { _mediator = mediator; _logger = logger; }

    [HttpPost("Login")]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto input, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(new LoginCommand(input), ct);
            var claims = new List<Claim> { //Issue cookie for server-side
                new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new(ClaimTypes.Email, result.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { _logger.LogWarning(ex, "Login fallido: {Message}", ex.Message); return Unauthorized(new[] { "Credenciales inválidas. Por favor, verifique su email y contraseña." }); }
        catch (Exception ex) { _logger.LogError(ex, "Error durante el login: {Message}", ex.Message); return StatusCode(500, new[] { "Ha ocurrido un error inesperado durante el inicio de sesión." }); }
    }

    [HttpPost("Refresh")]
    public ActionResult<string> Refresh(CancellationToken ct)
    {
        try
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized(new[] { "Usuario no autenticado" });
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); //Obtain user ID from claims
            var email = User.FindFirstValue(ClaimTypes.Email)!; //Ensure email is not null, throw exception if it is
            var token = HttpContext.RequestServices.GetRequiredService<Application.Security.IJwtGenerator>().Generate(userId, email);
            return Ok(token);
        }
        catch (Exception ex) { _logger.LogError(ex, "Error en refresh token: {Message}", ex.Message); return StatusCode(500, new[] { "Error al refrescar el token de autenticación." }); }
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        try { await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); return NoContent(); }
        catch (Exception ex) { _logger.LogError(ex, "Error durante el logout: {Message}", ex.Message); return StatusCode(500, new[] { "Error al cerrar la sesión." }); }
    }
}