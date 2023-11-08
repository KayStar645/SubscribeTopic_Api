using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class DetailThesisRequestHandler : IRequestHandler<DetailThesisRequest, Result<ThesisDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailThesisRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ThesisDto>> Handle(DetailThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Thesis>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
                    query = query.Include(x => x.ThesisInstructions);
                    query = query.Include(x => x.ThesisReviews);
                    query = query.Include(x => x.ThesisMajors);
                }
                else
                {
                    if (request.isGetIssuer == true)
                    {
                        query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
                    }

                    if (request.isGetThesisInstructions == true)
                    {
                        query = query.Include(x => x.ThesisInstructions);
                    }

                    if (request.isGetThesisReviews == true)
                    {
                        query = query.Include(x => x.ThesisReviews);
                    }

                    if (request.isGetThesisMajors == true)
                    {
                        query = query.Include(x => x.ThesisMajors);
                    }    
                }

                var findThesis = await query.SingleAsync();

                if (findThesis is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var thesisDto = _mapper.Map<ThesisDto>(findThesis);

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
