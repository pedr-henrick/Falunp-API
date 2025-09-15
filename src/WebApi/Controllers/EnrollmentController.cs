using Application.Common;
using Application.DTOs.Enrollment;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController(IEnrollmentService enrollmentService) : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;

        [HttpGet]
        [ProducesResponseType(typeof(Result<List<EnrollmentInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllEnrollmentAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _enrollmentService.GetAsync(cancellationToken);

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
                Title = "Get enrollment failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<List<EnrollmentInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEnrollmentAsync([FromBody] EnrollmentCreateDto enrollmentRequestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _enrollmentService.CreateAsync(enrollmentRequestDto, cancellationToken);

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
                Title = "Get enrollment failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }


        [HttpPatch]
        [ProducesResponseType(typeof(Result<List<EnrollmentInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEnrollmentAsync([FromBody] EnrollmentUpdateDto enrollmentUpdateDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _enrollmentService.UpdateAsync(enrollmentUpdateDto, cancellationToken);

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
                Title = "Get enrollment failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        [HttpDelete]
        [ProducesResponseType(typeof(Result<List<EnrollmentInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEnrollmentByClassAsync([FromQuery] EnrollmentFilterDto enrollmentFilterDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(modelStateDictionary: ModelState, statusCode: StatusCodes.Status400BadRequest);

            var result = await _enrollmentService.DeleteAsync(enrollmentFilterDto, cancellationToken);

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
                Title = "Get enrollment failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
