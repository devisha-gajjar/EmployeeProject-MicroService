namespace Employee.API.Extensions;

using Microsoft.AspNetCore.Mvc;

public static class ValidationResponseHelper
{
    public static IActionResult CreateValidationErrorResponse(ActionContext context, bool isDev)
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = new
        {
            Result = false,
            Message = "Validation failed",
            StatusCode = StatusCodes.Status400BadRequest,
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    }
}