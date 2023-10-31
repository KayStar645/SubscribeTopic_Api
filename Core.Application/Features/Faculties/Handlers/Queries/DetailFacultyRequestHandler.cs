using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Faculty;
using Core.Application.Features.Faculties.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Faculties.Handlers.Queries
{
    public class DetailFacultyRequestHandler : IRequestHandler<DetailFacultyRequest, Result<FacultyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailFacultyRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<FacultyDto>> Handle(DetailFacultyRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Domain.Entities.Faculties>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Domain.Entities.Faculties>().AddInclude(query, x => x.Dean_Teacher);
                    query = _unitOfWork.Repository<Domain.Entities.Faculties>().AddInclude(query, x => x.Departments);
                }
                else
                {
                    if (request.isGetDean == true)
                    {
                        query = _unitOfWork.Repository<Domain.Entities.Faculties>().AddInclude(query, x => x.Dean_Teacher);
                    }
                    if (request.isGetDepartment == true)
                    {
                        query = _unitOfWork.Repository<Domain.Entities.Faculties>().AddInclude(query, x => x.Departments);
                    }
                }

                var findFaculty = await query.SingleAsync();

                if (findFaculty is null)
                {
                    return Result<FacultyDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var facultyDto = _mapper.Map<FacultyDto>(findFaculty);

                return Result<FacultyDto>.Success(facultyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<FacultyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
