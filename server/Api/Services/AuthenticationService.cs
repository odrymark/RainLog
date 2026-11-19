using System.IdentityModel.Tokens.Jwt;
using Api.Common;
using Api.Dtos;
using Api.Helpers;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Api.Services;

public interface IAuthenticationService
{
    Task<ServiceResponse<(string Token, string Refresh)>> Login(LoginRequestDto dto);
}

public class AuthenticationService(AppDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration) : IAuthenticationService
{
    public async Task<ServiceResponse<(string Token, string Refresh)>> Login(LoginRequestDto dto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.Client.ShortName == dto.ClientShortName && u.Client.Active && u.Active);

        if (user == null)
            return ServiceResponse<(string Token, string Refresh)>.Failure("user_not_found", "Invalid login credentials.");

        if (dto.Password.Length > 255)
            return ServiceResponse<(string Token, string Refresh)>.Failure("password_too_long", "Maximum password length exceeded");

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return ServiceResponse<(string Token, string Refresh)>.Failure("user_not_found", "Invalid login credentials.");

        var tokens = await GetAccessToken(user);

        return ServiceResponse<(string Token, string Refresh)>.Success(tokens);
    }

    private async Task<(string AccessToken, string RefreshToken)> GetAccessToken(User user)
    {
        var currentUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (!string.IsNullOrEmpty(user.AccessToken) && TryGetJwtExpiryUnix(user.AccessToken, out long jwtExpiryUnix) && jwtExpiryUnix > currentUnix)
        {
            return (user.AccessToken, user.RefreshToken ?? string.Empty);
        }

        string accessToken = TokenGenerator.GenerateAccessToken(user.Id.ToString(), configuration);
        RefreshTokenResult refreshToken = TokenGenerator.GenerateRefreshToken(configuration);

        user.AccessToken = accessToken;
        user.RefreshToken = refreshToken.TokenHash;
        user.RefreshTokenExpiry = refreshToken.ExpiresAt;

        await context.SaveChangesAsync();

        return (accessToken, refreshToken.Token);
    }

    private static bool TryGetJwtExpiryUnix(string token, out long expiryUnix)
    {
        expiryUnix = 0;

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
            return false;

        var jwt = handler.ReadJwtToken(token);
        if (jwt.ValidTo == DateTime.MinValue)
            return false;

        expiryUnix = new DateTimeOffset(jwt.ValidTo, TimeSpan.Zero).ToUnixTimeSeconds();
        return true;
    }
}