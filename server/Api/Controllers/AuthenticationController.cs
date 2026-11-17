using Api.Common;
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
        var response = await authenticationService.Login(dto);

        if (response.ErrorCode != null)
            return this.HandleError(response.ErrorCode, response.ErrorMessage);

        return Ok(response.Data);
    }
}