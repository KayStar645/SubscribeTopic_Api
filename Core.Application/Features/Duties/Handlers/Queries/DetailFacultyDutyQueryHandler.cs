using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Features.Duties.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Queries
{
    public class DetailFacultyDutyQueryHandler : IRequestHandler<DetailFacultyDutyRequest, Result<FacultyDutyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailFacultyDutyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<FacultyDutyDto>> Handle(DetailFacultyDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDutyDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Duty>().GetByIdInclude(request.id)
                                        .Where(x => x.Type == Duty.TYPE_FACULTY);

                if (request.isAllDetail || request.isGetFaculty == true)
                {
                    query = query.Include(x => x.Faculty);
                }
                if (request.isAllDetail || request.isGetDepartment == true)
                {
                    query = query.Include(x => x.Department);
                }

                var findDuty = await query.SingleOrDefaultAsync();

                if (findDuty is null)
                {
                    return Result<FacultyDutyDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var FacultyDutyDto = _mapper.Map<FacultyDutyDto>(findDuty);

                return Result<FacultyDutyDto>.Success(FacultyDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
