using Core.Application.Models.Identity.Auths;
using Core.Application.Models.Identity.ViewModels;
using Core.Application.Responses;

namespace Core.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<Result<List<UserVM>>> GetList();
        Task<Result<AuthResponse>> Login(AuthRequest request);
        Task<Result<RegistrationResponse>> Register(RegistrationRequest request);

    }
}
