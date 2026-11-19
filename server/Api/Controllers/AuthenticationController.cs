using Api.Common;
using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController(IAuthenticationService authenticationService, IConfiguration configuration) : ControllerBase
{
    private readonly double _expiresInMinutes = configuration.GetSection("Jwt").GetValue("ExpiresInMinutes", 15.0);
    private readonly double _refreshExpireDays = configuration.GetSection("RefreshJwt").GetValue("ExpiresInDays", 3.0);
    
    private void SetTokenCookie(string token)
    {
        var isSecure = Request.IsHttps;
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(_expiresInMinutes)
        });
    }
    
    private void SetRefreshCookie(string refreshToken)
    {
        var isSecure = Request.IsHttps;
        
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(_refreshExpireDays)
        });
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var response = await authenticationService.Login(dto);

        if (response.ErrorCode != null)
            return this.HandleError(response.ErrorCode, response.ErrorMessage);
        
        SetTokenCookie(response.Data.Token);
        SetRefreshCookie(response.Data.Refresh);

        return Ok();
    }
}