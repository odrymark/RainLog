using Api.Dtos;

namespace Api.Services;

public interface IAuthenticationService
{
    Task<(string? token, string? errorCode, string? errorMessage)> Login(LoginRequestDto dto);
}

public class AuthenticationService : IAuthenticationService
{
    public async Task<(string? token, string? errorCode, string? errorMessage)> Login(LoginRequestDto dto)
    {
        throw new NotImplementedException();
    }
}