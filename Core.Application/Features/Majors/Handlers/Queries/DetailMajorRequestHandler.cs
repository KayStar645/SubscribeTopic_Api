using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Major;
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
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<MajorDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Major>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Industry);
                }
                else
                {
                    if (request.isGetIndustry == true)
                    {
                        query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Industry);
                    }
                }

                var findMajor = await query.SingleAsync();

                if (findMajor is null)
                {
                    return Result<MajorDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
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
