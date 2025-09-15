using Application.Common;
using Application.DTOs.Student;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentController(IStudentService studentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;

        [HttpGet]
        [ProducesResponseType(typeof(Result<List<StudentInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStudentesAsync([FromQuery] StudentRequestDto studentRequestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _studentService.GetAsync(studentRequestDto, cancellationToken);

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
                Title = "Get student failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateStudentesAsync([FromBody] StudentCreateDto studentCreateDto, CancellationToken cancellationToken)
        {
            var result = await _studentService.CreateAsync(studentCreateDto, cancellationToken);

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
                Title = "Get student failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStudentesAsync(Guid id, [FromBody] StudentUpdateDto studentUpdateDto, CancellationToken cancellationToken)
        {
            var result = await _studentService.Updatesync(id, studentUpdateDto, cancellationToken);

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
                Title = "Update student failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result<List<StudentInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudentesAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _studentService.DaleteAsync(id, cancellationToken);

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
                Title = "Update student failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
