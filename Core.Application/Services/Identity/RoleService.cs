using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Identity;
using Core.Application.Models.Identity.Auths;
using Core.Application.Models.Identity.Roles;
using Core.Application.Responses;
using Core.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Transactions;

namespace Core.Application.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<RoleResult>>> GetList()
        {
            try
            {
                var roles = await _unitOfWork.Repository<Role>()
                                             .GetAllAsync();

                var mapRoles = _mapper.Map<List<RoleResult>>(roles);

                return Result<List<RoleResult>>.Success(mapRoles, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<List<RoleResult>>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<RoleResult>> GetDetail(int pId)
        {
            try
            {
                var role = await _unitOfWork.Repository<Role>()
                                             .FirstOrDefaultAsync(x => x.Id == pId);

                var mapRole = _mapper.Map<RoleResult>(role);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                mapRole.PermissionsName = await _unitOfWork.Repository<RolePermission>()
                                              .GetAllInclude()
                                              .Where(x => x.RoleId == pId)
                                              .Include(x => x.Permission)
                                              .Select(x => x.Permission.Name)
                                              .ToListAsync();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

                return Result<RoleResult>.Success(mapRole, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<RoleResult>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<RoleResult>> CreateAsync(RoleRequest pRequest)
        {
            try
            {
                var role = _mapper.Map<Role>(pRequest);

                var result = await _unitOfWork.Repository<Role>().AddAsync(role);
                await _unitOfWork.Save(new CancellationToken());

                var roleResult = _mapper.Map<RoleResult>(result);

                if(pRequest.PermissionsName != null)
                {
                    foreach (string permissionName in pRequest.PermissionsName)
                    {
                        var permission = await _unitOfWork.Repository<Permission>()
                                                .FirstOrDefaultAsync(x => x.Name == permissionName);

                        if(permission != null)
                        {

                            var per = new RolePermission
                            {
                                RoleId = role.Id,
                                PermissionId = permission.Id
                            };
                            await _unitOfWork.Repository<RolePermission>()
                                .AddAsync(per);
                        }  
                    }
                }    

                await _unitOfWork.Save(new CancellationToken());

                return Result<RoleResult>.Success(roleResult, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<RoleResult>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<RoleResult>> UpdateAsync(RoleRequest pRequest)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var role = _mapper.Map<Role>(pRequest);
                    var result = await _unitOfWork.Repository<Role>().UpdateAsync(role);
                    await _unitOfWork.Save(new CancellationToken());

                    // Get Permission hiện của của Role này
                    var currentPermissionsName = await _unitOfWork.Repository<RolePermission>()
                                                        .GetAllInclude()
                                                        .Include(x => x.Permission)
                                                        .Where(x => x.RoleId == pRequest.Id)
                                                        .Select(x => x.Permission.Name)
                                                        .ToListAsync();

                    if(pRequest.PermissionsName != null)
                    {
                        var deletePermission = currentPermissionsName.Except(pRequest.PermissionsName).ToList();
                        var addPermission = pRequest.PermissionsName.Except(currentPermissionsName).ToList();

                        foreach (string permissionName in deletePermission)
                        {
                            var del = await _unitOfWork.Repository<RolePermission>()
                                        .Query()
                                        .Include(x => x.Permission)
                                        .Where(x => x.RoleId == role.Id && x.Permission.Name == permissionName)
                                        .FirstOrDefaultAsync();
                            if(del != null)
                            {
                                await _unitOfWork.Repository<RolePermission>()
                                .DeleteAsync(del);
                            }    
                        }

                        foreach (string permissionName in addPermission)
                        {
                            var permission = await _unitOfWork.Repository<Permission>()
                                                 .FirstOrDefaultAsync(x => x.Name == permissionName);

                            if (permission != null)
                            {

                                var per = new RolePermission
                                {
                                    RoleId = role.Id,
                                    PermissionId = permission.Id
                                };
                                await _unitOfWork.Repository<RolePermission>()
                                    .AddAsync(per);
                            }
                        }

                        await _unitOfWork.Save(new CancellationToken());
                    }    
                    
                    transaction.Complete();

                    var roleResult = _mapper.Map<RoleResult>(result);
                    return Result<RoleResult>.Success(roleResult, (int)HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return Result<RoleResult>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                }
            }    
        }

        public async Task DeleteAsync(int pId)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Get Permission hiện của của Role này
                    var currentPermissionsId = await _unitOfWork.Repository<RolePermission>()
                                                        .GetAllInclude(x => x.Role)
                                                        .Where(x => x.RoleId == pId)
                                                        .Select(x => x.PermissionId)
                                                        .ToListAsync();

                    foreach (int permissionId in currentPermissionsId)
                    {
                        var del = await _unitOfWork.Repository<RolePermission>()
                            .FirstOrDefaultAsync(x => x.RoleId == pId && x.PermissionId == permissionId);

                        await _unitOfWork.Repository<RolePermission>()
                            .DeleteAsync(del);
                    }

                    var role = await _unitOfWork.Repository<Role>().FirstOrDefaultAsync(x => x.Id == pId);
                    await _unitOfWork.Repository<Role>().DeleteAsync(role);

                    await _unitOfWork.Save(new CancellationToken());
                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                }
            }
        }

        public async Task<Result<RoleResult>> AssignRoles(AssignRoleRequest pRequest)
        {
            var result = await _unitOfWork.Repository<UserRole>().AddAsync(new UserRole
            {
                UserId = pRequest.UserId,
                RoleId = pRequest.RoleId
            });
            await _unitOfWork.Save(new CancellationToken());

            return await GetDetail((int)result.RoleId);
        }
    }
}
