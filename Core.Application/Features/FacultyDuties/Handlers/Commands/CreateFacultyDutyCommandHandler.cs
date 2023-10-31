using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.FacultyDuty.Validators;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.FacultyDuties.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.FacultyDuties.Handlers.Commands
{
    public class CreateFacultyDutyCommandHandler : IRequestHandler<CreateFacultyDutyRequest, Result<FacultyDutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateFacultyDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FacultyDutyDto>> Handle(CreateFacultyDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateFacultyDutyDtoValidator(_unitOfWork,
                request.CreateFacultyDutyDto.FacultyId,
                request.CreateFacultyDutyDto.TimeStart ?? DateTime.Now);
            var validatorResult = await validator.ValidateAsync(request.CreateFacultyDutyDto);

            if (validatorResult.IsValid == false)
            {
                var errorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var FacultyDuty = _mapper.Map<FacultyDuty>(request.CreateFacultyDutyDto);

                var newFacultyDuty = await _unitOfWork.Repository<FacultyDuty>().AddAsync(FacultyDuty);
                await _unitOfWork.Save(cancellationToken);

                var FacultyDutyDto = _mapper.Map<FacultyDutyDto>(newFacultyDuty);

                return Result<FacultyDutyDto>.Success(FacultyDutyDto, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
