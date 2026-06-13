using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Api.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.Success) return new OkResult();
        return result.Error?.Code switch
        {
            "NOT_FOUND"    => new NotFoundObjectResult(result),
            "UNAUTHORIZED" => new UnauthorizedObjectResult(result),
            "FORBIDDEN"    => new ObjectResult(result) { StatusCode = 403 },
            "BAD_REQUEST"  => new BadRequestObjectResult(result),
            _              => new ObjectResult(result) { StatusCode = 500 },
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, bool created = false)
    {
        if (result.Success)
            return created ? new ObjectResult(result) { StatusCode = 201 } : new OkObjectResult(result);
        return result.Error?.Code switch
        {
            "NOT_FOUND"    => new NotFoundObjectResult(result),
            "UNAUTHORIZED" => new UnauthorizedObjectResult(result),
            "FORBIDDEN"    => new ObjectResult(result) { StatusCode = 403 },
            "BAD_REQUEST"  => new BadRequestObjectResult(result),
            _              => new ObjectResult(result) { StatusCode = 500 },
        };
    }
}
