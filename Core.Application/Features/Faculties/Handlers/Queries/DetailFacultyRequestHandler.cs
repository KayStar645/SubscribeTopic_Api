using AutoMapper;
using Core.Application.Contracts.Persistence;
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
            try
            {
                var query = _unitOfWork.Repository<Faculty>().GetByIdInclude(request.Id);

                if (request.IsAllDetail)
                {
                    
                }
                else
                {
                    
                }

                var findFaculty = await query.SingleAsync();

                if (findFaculty is null)
                {
                    return Result<FacultyDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.Id.ToString()),
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
