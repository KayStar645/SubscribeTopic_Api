using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Majors.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Majors.Handlers.Queries
{
    public class DetailMajorRequestHandler : IRequestHandler<DetailMajorRequest, Result<MajorDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailMajorRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<MajorDto>> Handle(DetailMajorRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Repository<Major>().GetByIdInclude(request.Id);

                if (request.IsAllDetail)
                {
                    query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Faculty);
                }
                else
                {
                    if (request.isGetFaculties == true)
                    {
                        query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Faculty);
                    }
                }

                var findMajor = await query.SingleAsync();

                if (findMajor is null)
                {
                    return Result<MajorDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var majorDto = _mapper.Map<MajorDto>(findMajor);

                return Result<MajorDto>.Success(majorDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<MajorDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
