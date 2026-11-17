using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Helpers;

public static class AccessTokenGenerator
{
    public static string GenerateToken(int size = 32)
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(size);

        return WebEncoders.Base64UrlEncode(randomBytes);
    }
}