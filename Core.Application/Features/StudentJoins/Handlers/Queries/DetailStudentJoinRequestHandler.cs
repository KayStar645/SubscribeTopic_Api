using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.StudentJoin;
using Core.Application.Features.StudentJoins.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.StudentJoins.Handlers.Queries
{
    public class DetailStudentJoinRequestHandler : IRequestHandler<DetailStudentJoinRequest, Result<StudentJoinDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailStudentJoinRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StudentJoinDto>> Handle(DetailStudentJoinRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentJoinDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<StudentJoin>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.Student);
                    query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.RegistrationPeriod);
                }
                else
                {
                    if(request.isGetStudent)
                    {
                        query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.Student);
                    }

                    if(request.isGetRegistrationPeriod)
                    {
                        query = _unitOfWork.Repository<StudentJoin>().AddInclude(query, x => x.RegistrationPeriod);
                    }
                }

                var findStudentJoin = await query.SingleAsync();

                if (findStudentJoin is null)
                {
                    return Result<StudentJoinDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var studentJoinDto = _mapper.Map<StudentJoinDto>(findStudentJoin);

                return Result<StudentJoinDto>.Success(studentJoinDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
