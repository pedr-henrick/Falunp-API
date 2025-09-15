using Application.Common;
using Application.DTOs.Class;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController(IClassService classService) : ControllerBase
    {
        private readonly IClassService _classService = classService;

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(Result<List<ClassInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllClassesAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _classService.GetAllAsync(cancellationToken);

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
                Title = "Get class failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
