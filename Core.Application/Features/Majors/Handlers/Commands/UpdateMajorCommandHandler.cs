using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Major.Validators;
using Core.Application.Features.Majors.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Majors.Handlers.Commands
{
    public class UpdateMajorCommandHandler : IRequestHandler<UpdateMajorRequest, Result<MajorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMajorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<MajorDto>> Handle(UpdateMajorRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateMajorDtoValidator(_unitOfWork, request.updateMajorDto.Id);
            var validationResult = await validator.ValidateAsync(request.updateMajorDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<MajorDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findMajor = await _unitOfWork.Repository<Major>().GetByIdAsync(request.updateMajorDto.Id);

                if (findMajor is null)
                {
                    return Result<MajorDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.updateMajorDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findMajor.CopyPropertiesFrom(request.updateMajorDto);

                var newMajor = await _unitOfWork.Repository<Major>().UpdateAsync(findMajor);
                await _unitOfWork.Save(cancellationToken);

                var majorDto = _mapper.Map<MajorDto>(newMajor);

                return Result<MajorDto>.Success(majorDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<MajorDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
