using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Support.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return new UnauthorizedObjectResult(new { message = result.Error });
        }
    }
}
