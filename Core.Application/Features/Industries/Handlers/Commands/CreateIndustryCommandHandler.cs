using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Industry;
using Core.Application.DTOs.Industry.Validators;
using Core.Application.Features.Industries.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Industries.Handlers.Commands
{
    public class CreateIndustryCommandHandler : IRequestHandler<CreateIndustryRequest, Result<IndustryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateIndustryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IndustryDto>> Handle(CreateIndustryRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateIndustryDtoValidator(_unitOfWork);
            var validatorResult = await validator.ValidateAsync(request.createIndustryDto);

            if(validatorResult.IsValid == false)
            {
                var errorMessages  = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<IndustryDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var industry = _mapper.Map<Industry>(request.createIndustryDto);

                var newIndustry = await _unitOfWork.Repository<Industry>().AddAsync(industry);
                await _unitOfWork.Save(cancellationToken);

                var industryDto = _mapper.Map<IndustryDto>(newIndustry);

                return Result<IndustryDto>.Success(industryDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
