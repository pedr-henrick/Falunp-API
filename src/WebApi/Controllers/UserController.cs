using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _authService.AuthenticateAsync(request);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.ValidationErrors.Count != 0)
            {
                foreach (var error in result.ValidationErrors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                
                return ValidationProblem(ModelState);
            }

            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}