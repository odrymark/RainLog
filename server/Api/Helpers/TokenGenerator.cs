using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace Api.Helpers;

public static class TokenGenerator
{
    public static string GenerateAccessToken(string userId, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");

        var secretKey = jwtSection["SecretKey"]
                        ?? throw new InvalidOperationException("Jwt:SecretKey is missing from configuration.");
        var issuer = jwtSection["Issuer"]
                     ?? throw new InvalidOperationException("Jwt:Issuer is missing from configuration.");
        var audience = jwtSection["Audience"]
                       ?? throw new InvalidOperationException("Jwt:Audience is missing from configuration.");

        var expiresInMinutes = jwtSection.GetValue<int?>("ExpiresInMinutes") ?? 15;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static RefreshTokenResult GenerateRefreshToken(IConfiguration configuration, int size = 32)
    {
        var refreshSection = configuration.GetSection("RefreshJwt");
        var expiresInDays = refreshSection.GetValue<int?>("ExpiresInDays") ?? 5;

        byte[] randomBytes = RandomNumberGenerator.GetBytes(size);
        string token = WebEncoders.Base64UrlEncode(randomBytes);

        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        string tokenHash = Convert.ToBase64String(hashBytes);

        long expiresAtUnix = DateTimeOffset.UtcNow.AddDays(expiresInDays).ToUnixTimeSeconds();

        return new RefreshTokenResult(
            Token: token,
            TokenHash: tokenHash,
            ExpiresAt: expiresAtUnix
        );
    }
}