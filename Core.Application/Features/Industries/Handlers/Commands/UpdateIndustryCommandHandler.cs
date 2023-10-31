using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Industry;
using Core.Application.DTOs.Industry.Validators;
using Core.Application.Features.Industries.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Industries.Handlers.Commands
{
    public class UpdateIndustryCommandHandler : IRequestHandler<UpdateIndustryRequest, Result<IndustryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateIndustryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IndustryDto>> Handle(UpdateIndustryRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateIndustryDtoValidator(_unitOfWork, request.updateIndustryDto.Id);
            var validationResult = await validator.ValidateAsync(request.updateIndustryDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<IndustryDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findIndustry = await _unitOfWork.Repository<Industry>().GetByIdAsync(request.updateIndustryDto.Id);

                if (findIndustry is null)
                {
                    return Result<IndustryDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.updateIndustryDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findIndustry.CopyPropertiesFrom(request.updateIndustryDto);

                var newIndustry = await _unitOfWork.Repository<Industry>().UpdateAsync(findIndustry);
                await _unitOfWork.Save(cancellationToken);

                var industryDto = _mapper.Map<IndustryDto>(newIndustry);

                return Result<IndustryDto>.Success(industryDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
