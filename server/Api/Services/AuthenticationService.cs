using Api.Common;
using Api.Dtos;
using Api.Helpers;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Api.Services;

public interface IAuthenticationService
{
    Task<ServiceResponse<string>> Login(LoginRequestDto dto);
}

public class AuthenticationService(AppDbContext context, IPasswordHasher<User> passwordHasher) : IAuthenticationService
{
    private static readonly TimeSpan TokenExpiry = TimeSpan.FromHours(24);
    
    public async Task<ServiceResponse<string>> Login(LoginRequestDto dto)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.Client.ShortName == dto.ClientShortName && u.Client.Active && u.Active);

        if (user == null)
            return ServiceResponse<string>.Failure("user_not_found", "Invalid login credentials.");

        if (dto.Password.Length > 255)
            return ServiceResponse<string>.Failure("password_too_long", "Maximum password length exceeded");

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return ServiceResponse<string>.Failure("user_not_found", "Invalid login credentials.");

        var token = await GetAccessToken(user);

        return ServiceResponse<string>.Success(token);
    }

    private async Task<string> GetAccessToken(User user)
    {
        var currentUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (user.AccessToken != null && user.TokenExpiry > currentUnix)
            return user.AccessToken;

        string token = AccessTokenGenerator.GenerateToken();
        long expiry = DateTimeOffset.UtcNow.Add(TokenExpiry).ToUnixTimeSeconds();

        user.AccessToken = token;
        user.TokenExpiry = expiry;

        return token;
    }
}