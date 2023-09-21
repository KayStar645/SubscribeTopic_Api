using Core.Application.Models.Identity;
using Core.Application.Responses;

namespace Core.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> Login(AuthRequest request);
        Task<Result<RegistrationResponse>> Register(RegistrationRequest request);

    }
}
