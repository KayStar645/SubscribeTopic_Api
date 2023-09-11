﻿using Core.Application.Constants;
using Core.Application.Contracts.Identity;
using Core.Application.Models.Identity;
using Core.Application.Models.Identity.Validators;
using Core.Application.Transform;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
                permissionClaims.Add(new Claim(CustomClaimTypes.Permission, roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(CustomClaimTypes.Uid, user.Id) 
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
