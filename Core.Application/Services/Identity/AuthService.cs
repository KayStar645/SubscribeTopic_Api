using Core.Application.Constants;
using Core.Application.Contracts.Identity;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models.Identity;
using Core.Application.Models.Identity.Validators;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Core.Application.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepo;

        public AuthService(JwtSettings jwtSettings, IUserRepository userRepository)
        {
            _jwtSettings = jwtSettings;
            _userRepo = userRepository;
        }    

        public async Task<Result<AuthResponse>> Login(AuthRequest request)
        {
            try
            {
                User user = await _userRepo.FindByNameAsync(request.UserName);

                if (user == null)
                {
                    return Result<AuthResponse>
                        .Failure(IdentityTranform.UserNotExists(request.UserName),
                    (int)HttpStatusCode.BadRequest);
                }

                bool result = await _userRepo.PasswordSignInAsync(user.UserName, request.Password);

                if (result == false)
                {
                    return Result<AuthResponse>
                        .Failure(IdentityTranform.InvalidCredentials(request.UserName),
                        (int)HttpStatusCode.BadRequest);
                }

                JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

                AuthResponse auth = new AuthResponse
                {
                    Id = user.Id.ToString(),
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    UserName = user.UserName
                };

                return Result<AuthResponse>.Success(auth, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<RegistrationResponse>> Register(RegistrationRequest request)
        {
            try
            {
                RegistrationRequestValidator validator = new RegistrationRequestValidator();
                var validationResult = await validator.ValidateAsync(request);

                if (validationResult.IsValid == false)
                {
                    return Result<RegistrationResponse>
                        .Failure(validationResult.Errors.Select(q => q.ErrorMessage).First(),
                        (int)HttpStatusCode.BadRequest);
                }

                User user = await _userRepo.FindByNameAsync(request.UserName);

                if (user != null)
                {
                    return Result<RegistrationResponse>
                        .Failure(IdentityTranform.UserAlreadyExists(request.UserName),
                        (int)HttpStatusCode.BadRequest);
                }

                var newUser = new User
                {
                    UserName = request.UserName,
                };

                var result = await _userRepo.CreateAsync(new User(request.UserName, request.UserName));

                if (result)
                {
                    return Result<RegistrationResponse>
                        .Success(new RegistrationResponse() { UserName = newUser.UserName },
                        (int)HttpStatusCode.Created);
                }
                else
                {
                    // Chưa làm biến dịch
                    return Result<RegistrationResponse>.Failure("Tạo không thành công!", (int)HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Result<RegistrationResponse>.Failure(ex.Message,
                    (int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(User user)
        {
            var roles = await _userRepo.GetRolesAsync(user);
            var permissions = await _userRepo.GetPermissionsAsync(user);

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role.Name));
            var permissionClaims = permissions.Select(permission => new Claim(CONSTANT_CLAIM_TYPES.Permission, permission.Name));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(CONSTANT_CLAIM_TYPES.Uid, user.Id.ToString())
            }
            .Union(permissionClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

    }
}
