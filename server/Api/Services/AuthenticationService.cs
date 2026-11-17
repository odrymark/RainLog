using Api.Common;
using Api.Dtos;

namespace Api.Services;

public interface IAuthenticationService
{
    Task<ServiceResponse<string>> Login(LoginRequestDto dto);
}

public class AuthenticationService : IAuthenticationService
{
    public async Task<ServiceResponse<string>> Login(LoginRequestDto dto)
    {
        throw new NotImplementedException();
    }
}