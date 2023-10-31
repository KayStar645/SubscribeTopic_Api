using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Major.Validators;
using Core.Application.Features.Majors.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Majors.Handlers.Commands
{
    public class CreateMajorCommandHandler : IRequestHandler<CreateMajorRequest, Result<MajorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateMajorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<MajorDto>> Handle(CreateMajorRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateMajorDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createMajorDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<MajorDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var major = _mapper.Map<Major>(request.createMajorDto);

                var newMajor = await _unitOfWork.Repository<Major>().AddAsync(major);
                await _unitOfWork.Save(cancellationToken);

                var majorDto = _mapper.Map<MajorDto>(newMajor);

                return Result<MajorDto>.Success(majorDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<MajorDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
