using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Server.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment environment)
    { _logger = logger; _environment = environment; }

    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;
        var problemDetails = new ProblemDetails
        {
            Status = 500,
            Title = "An unexpected error occurred",
            Instance = HttpContext.TraceIdentifier,
            Detail = _environment.IsDevelopment() ? exception?.ToString() : "An internal server error has occurred.",
        };

        if (exception != null) { _logger.LogError(exception, "An unhandled exception occurred"); }
        return StatusCode(500, problemDetails);
    }

    [Route("/error-development")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleErrorDevelopment()
    {
        if (!_environment.IsDevelopment()) return NotFound();
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;
        var problemDetails = new ProblemDetails
        {
            Status = 500,
            Detail = exception?.ToString(),
            Title = "An unexpected error occurred",
            Instance = HttpContext.TraceIdentifier,
        };
        return StatusCode(500, problemDetails);
    }
}