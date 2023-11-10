using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.FacultyDuties.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.FacultyDuties.Handlers.Queries
{
    public class DetailFacultyDutyRequestHandler : IRequestHandler<DetailFacultyDutyRequest, Result<FacultyDutyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailFacultyDutyRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
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
                var query = _unitOfWork.Repository<FacultyDuty>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Faculty);
                    query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Department);
                }
                else
                {
                    if (request.isGetFaculty == true)
                    {
                        query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Faculty);
                    }
                    if (request.isGetDepartment == true)
                    {
                        query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Department);
                    }
                }

                var findFacultyDuty = await query.SingleOrDefaultAsync();

                if (findFacultyDuty is null)
                {
                    return Result<FacultyDutyDto>.Failure(
                        ValidatorTransform.NotExistsValue("id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var FacultyDutyDto = _mapper.Map<FacultyDutyDto>(findFacultyDuty);

                return Result<FacultyDutyDto>.Success(FacultyDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
