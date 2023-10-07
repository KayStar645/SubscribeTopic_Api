using Core.Application.Constants;
using Core.Application.Contracts.Identity;
using Core.Application.Models.Identity;
using Core.Application.Models.Identity.Validators;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Core.Application.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<Users> userManager,
            IOptions<JwtSettings> jwtSettings,
            SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
        }

        public async Task<Result<AuthResponse>> Login(AuthRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);

                if (user == null)
                {
                    return Result<AuthResponse>
                        .Failure(IdentityTranform.UserNotExists(request.UserName),
                        (int)HttpStatusCode.BadRequest);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

                if (result.Succeeded == false)
                {
                    return Result<AuthResponse>
                        .Failure(IdentityTranform.InvalidCredentials(request.UserName),
                        (int)HttpStatusCode.BadRequest);
                }

                JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

                AuthResponse auth = new AuthResponse
                {
                    Id = user.Id,
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

                var existingUser = await _userManager.FindByNameAsync(request.UserName);

                if (existingUser != null)
                {
                    return Result<RegistrationResponse>
                        .Failure(IdentityTranform.UserAlreadyExists(request.UserName),
                        (int)HttpStatusCode.BadRequest);
                }

                var user = new Users
                {
                    UserName = request.UserName,
                };

                var existingUserName = await _userManager.FindByEmailAsync(request.UserName);

                if (existingUserName == null)
                {
                    var result = await _userManager.CreateAsync(user, request.Password);

                    if (result.Succeeded)
                    {
                        // Add quyền nè
                        //await _userManager.AddToRoleAsync(user, RoleConfig.Ministry());

                        return Result<RegistrationResponse>
                            .Success(new RegistrationResponse() { UserId = user.Id },
                            (int)HttpStatusCode.Created);
                    }
                    else
                    {
                        return Result<RegistrationResponse>
                        .Failure(result.Errors.First().ToString(),
                        (int)HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Result<RegistrationResponse>
                        .Failure(IdentityTranform.UserAlreadyExists(request.UserName),
                        (int)HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Result<RegistrationResponse>.Failure(ex.Message,
                    (int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(Users user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userManager.GetClaimsAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));
            }

            var permissionClaims = new List<Claim>();

            for (int i = 0; i < permissions.Count; i++)
            {
                permissionClaims.Add(new Claim(ConstantClaimTypes.Permission, roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ConstantClaimTypes.Uid, user.Id) 
            }
            .Union(permissionClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
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
