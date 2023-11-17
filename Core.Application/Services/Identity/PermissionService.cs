using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Identity;
using Core.Application.Responses;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Reflection;

namespace Core.Application.Services.Identity
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<string>>> GetList(Assembly pAssembly)
        {
            try
            {
                var roles = new List<string>();

                var controllerTypes = pAssembly.GetTypes()
                    .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

                foreach (var controllerType in controllerTypes)
                {
                    var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(method => method.IsDefined(typeof(AuthorizeAttribute), inherit: true));

                    foreach (var method in methods)
                    {
                        var authorizeAttribute = method.GetCustomAttribute<AuthorizeAttribute>();
                        if (authorizeAttribute != null)
                        {
                            var rolesInAttribute = authorizeAttribute.Roles.Split(',');
                            roles.AddRange(rolesInAttribute);
                        }
                    }
                }

                roles = roles.Distinct().ToList();

                return Result<List<string>>.Success(roles, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<List<string>>> Create(List<string> pPermissions)
        {
            try
            {
                foreach (var permission in pPermissions)
                {
                    var per = await _unitOfWork.Repository<Permission>().FirstOrDefaultAsync(x => x.Name == permission);

                    if (per == null)
                    {
                        await _unitOfWork.Repository<Permission>().AddAsync(new Permission
                        {
                            Name = permission
                        });
                    }
                }
                await _unitOfWork.Save(new CancellationToken());

                var permissions = await _unitOfWork.Repository<Permission>()
                                                   .GetAllInclude()
                                                   .Select(x => x.Name)
                                                   .ToListAsync();

                return Result<List<string>>.Success(permissions, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
