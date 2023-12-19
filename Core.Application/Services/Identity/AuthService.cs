﻿using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Identity;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Teacher;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models.Identity.Auths;
using Core.Application.Models.Identity.Roles;
using Core.Application.Models.Identity.Validators;
using Core.Application.Models.Identity.ViewModels;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Core.Application.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork,
            IConfiguration configuration, IMapper mapper)
        {
            _userRepo = userRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<Result<List<UserVM>>> GetList()
        {
            var users = await _userRepo.Get();

            var mapUsers = _mapper.Map<List<UserVM>>(users);

            foreach(var user in mapUsers)
            {
                if(user.Type == User.TYPE_STUDENT)
                {
                    var student = await _unitOfWork.Repository<Student>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == user.UserName);
                    user.Student = _mapper.Map<StudentDto>(student);
                }
                else if(user.Type == User.TYPE_TEACHER)
                {
                    var teacher = await _unitOfWork.Repository<Teacher>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == user.UserName);

                    user.Teacher = _mapper.Map<TeacherDto>(teacher);
                }
                var mapUser = _mapper.Map<User>(user);
                var roles = await _userRepo.GetRolesAsync(mapUser);
                if(roles != null)
                {
                    user.Roles = _mapper.Map<List<RoleResult>>(roles);
                }    
            }

            return Result<List<UserVM>>.Success(mapUsers, (int)HttpStatusCode.OK);
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

                var result = await _userRepo.CreateAsync(new User(request.UserName, request.Password));

                if (result)
                {
                    return Result<RegistrationResponse>
                        .Success(new RegistrationResponse() { UserName = request.UserName },
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
            var result = await _userRepo.GetFacultyAsync(user);
            var facultyDto = _mapper.Map<FacultyDto>(result.faculty);

            var customer = await _userRepo.GetCustomerByUserName(user.UserName, result.type);
            string type = CLAIMS_VALUES.TYPE_ADMIN;
            if (result.type == 0)
            {
                type = CLAIMS_VALUES.TYPE_STUDENT;
                customer = _mapper.Map<StudentDto>(customer);
            }    
            else if (result.type == 1)
            {
                type = CLAIMS_VALUES.TYPE_TEACHER;
                customer = _mapper.Map<TeacherDto>(customer);
            }

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role.Name));
            var permissionClaims = permissions.Select(permission => new Claim(CONSTANT_CLAIM_TYPES.Permission, permission.Name));

            var pCustomer = customer == null ? "" : JsonSerializer.Serialize(customer,
                                    new JsonSerializerOptions
                                    {
                                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                                    });
            var pFaculty = facultyDto == null ? "" : JsonSerializer.Serialize(facultyDto,
                                    new JsonSerializerOptions
                                    {
                                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                                    });
            var pFacultyId = facultyDto == null ? "" : facultyDto?.Id?.ToString();

            var claims = new[]
            {
                new Claim(CONSTANT_CLAIM_TYPES.Uid, user.Id.ToString()),
                new Claim(CONSTANT_CLAIM_TYPES.UserName, user.UserName),
                new Claim(CONSTANT_CLAIM_TYPES.Type, type),
                new Claim(CONSTANT_CLAIM_TYPES.FacultyId, pFacultyId),
                new Claim(CONSTANT_CLAIM_TYPES.Customer, pCustomer),
                new Claim(CONSTANT_CLAIM_TYPES.Faculty, pFaculty),
            }
            .Union(permissionClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

    }
}
