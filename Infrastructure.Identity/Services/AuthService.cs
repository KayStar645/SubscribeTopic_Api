using Core.Application.Config;
using Core.Application.Constants;
using Core.Application.Contracts.Identity;
using Core.Application.Custom;
using Core.Application.Models.Identity;
using Core.Application.Models.Identity.Validators;
using Core.Application.Transform;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<IdentityUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
        }

        public async Task<AuthResponse> Login(AuthRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                throw new HttpRequestException(IdentityTranform.UserNotExists(request.UserName));
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (result.Succeeded == false)
            {
                throw new HttpRequestException(IdentityTranform.InvalidCredentials(request.UserName));
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            AuthResponse response = new AuthResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };

            return response;
        }

        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            RegistrationRequestValidator validator = new RegistrationRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.IsValid == false)
            {
                throw new HttpRequestException(validationResult.Errors.Select(q => q.ErrorMessage).First());
            }

            var existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
            {
                throw new HttpRequestException(IdentityTranform.UserAlreadyExists(request.UserName));
            }

            var user = new IdentityUser
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
                    return new RegistrationResponse() { UserId = user.Id };
                }
                else
                {
                    throw new Exception($"{result.Errors}");
                }
            }
            else
            {
                throw new HttpRequestException(IdentityTranform.UserAlreadyExists(request.UserName));
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
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
