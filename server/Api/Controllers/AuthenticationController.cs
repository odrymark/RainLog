using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var (token, errorCode, errorMessage) = await authenticationService.Login(dto);

        if (errorCode != null)
            return HandleError(errorCode, errorMessage);

        return Ok(token);
    }
    
    private ActionResult HandleError(
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
            "user_not_found" or "route_not_found" or "order_not_found" or "vehicle_not_found"
                or "order_status_not_found" or "orderline_not_found" or "status_type_not_found" or "client_not_found" => NotFound(response),

            "order_vehicle_not_assigned" => Conflict(response),
            "invalid_order" or "image_too_large" or "invalid_file_type" or "occured_not_set" or "occured_invalid_range" or "too_many_images" => BadRequest(response),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                response)
        };
    }
}