using Application.Common;
using Application.DTOs.Class;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassController(IClassService classService) : ControllerBase
    {
        private readonly IClassService _classService = classService;

        [HttpGet]
        [ProducesResponseType(typeof(Result<List<ClassInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllClassesAsync([FromQuery] ClassFilterDto classFilterDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _classService.GetAsync(classFilterDto, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.ValidationErrors.Count != 0)
            {
                foreach (var error in result.ValidationErrors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);
            }

            return Unauthorized(new ProblemDetails
            {
                Title = "Get class failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateClassAsync([FromBody] ClassDto classDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _classService.CreateAsync(classDto, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.ValidationErrors.Count != 0)
            {
                foreach (var error in result.ValidationErrors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);
            }

            return Unauthorized(new ProblemDetails
            {
                Title = "Add class failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateClassAsync(Guid id, [FromBody] ClassDto classDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _classService.UpdateAsync(id, classDto, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.ValidationErrors.Count != 0)
            {
                foreach (var error in result.ValidationErrors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);
            }

            return Unauthorized(new ProblemDetails
            {
                Title = "Update class failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteClassAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _classService.DeleteAsync(id, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.ValidationErrors.Count != 0)
            {
                foreach (var error in result.ValidationErrors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);
            }

            return Unauthorized(new ProblemDetails
            {
                Title = "Update class failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
