using Microsoft.AspNetCore.Mvc;

namespace Api.Common;

public static class ErrorHandlerExtensions
{
    public static ActionResult HandleError(
        this ControllerBase controller,
        string? errorCode,
        string? errorMessage)
    {
        var response = new
        {
            code = errorCode,
            message = errorMessage
        };

        return errorCode switch
        {
            "user_not_found" => controller.NotFound(response),

            "conflict" => controller.Conflict(response),

            "password_too_long" => controller.BadRequest(response),

            _ => controller.StatusCode(
                StatusCodes.Status500InternalServerError,
                response)
        };
    }
}