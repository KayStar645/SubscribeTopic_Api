using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.DTOs.StudentJoin;
using Core.Application.Features.Groups.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Groups.Handlers.Queries
{
    public class DetailGroupRequestHandler : IRequestHandler<DetailGroupRequest, Result<GroupDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailGroupRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GroupDto>> Handle(DetailGroupRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<GroupDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Group>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Group>().AddInclude(query, x => x.Leader.Student);
                }
                else
                {
                    if (request.isGetLeader == true)
                    {
                        query = _unitOfWork.Repository<Group>().AddInclude(query, x => x.Leader.Student);
                    }
                }

                var findGroup = await query.SingleAsync();

                if (findGroup is null)
                {
                    return Result<GroupDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var groupDto = _mapper.Map<GroupDto>(findGroup);

                if (request.isGetMember == true || request.isAllDetail == true)
                {
                    var members = await _unitOfWork.Repository<StudentJoin>()
                                        .Query()
                                        .Where(x => x.GroupId == findGroup.Id)
                                        .Include(x => x.Student)
                                        .ToListAsync();

                    groupDto.members = _mapper.Map<List<StudentJoinDto>>(members);
                }

                return Result<GroupDto>.Success(groupDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<GroupDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
