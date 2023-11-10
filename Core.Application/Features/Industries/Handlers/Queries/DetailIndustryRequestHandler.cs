using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Industry;
using Core.Application.Features.Industries.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;

namespace Core.Application.Features.Industries.Handlers.Queries
{
    public class DetailIndustryRequestHandler : IRequestHandler<DetailIndustryRequest, Result<IndustryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailIndustryRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IndustryDto>> Handle(DetailIndustryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new DetailBaseRequestValidator();
                var result = await validator.ValidateAsync(request);

                if (result.IsValid == false)
                {
                    var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return Result<IndustryDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
                }

                var query = _unitOfWork.Repository<Industry>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Industry>().AddInclude(query, x => x.Faculty);
                }
                else
                {
                    if (request.isGetFaculty == true)
                    {
                        query = _unitOfWork.Repository<Industry>().AddInclude(query, x => x.Faculty);
                    }
                }

                var findIndustry = await query.SingleAsync();

                if (findIndustry is null)
                {
                    return Result<IndustryDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var IndustryDto = _mapper.Map<IndustryDto>(findIndustry);

                return Result<IndustryDto>.Success(IndustryDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
