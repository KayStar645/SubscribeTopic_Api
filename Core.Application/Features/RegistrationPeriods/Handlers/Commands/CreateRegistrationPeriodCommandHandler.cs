using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.RegistrationPeriod.Validators;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Commands
{
    public class CreateRegistrationPeriodCommandHandler : IRequestHandler<CreateRegistrationPeriodRequest, Result<RegistrationPeriodDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRegistrationPeriodRepository _registrationPeriodRepo;
        private readonly IMapper _mapper;

        public CreateRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IRegistrationPeriodRepository registrationPeriodRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _registrationPeriodRepo = registrationPeriodRepository;
        }

        public async Task<Result<RegistrationPeriodDto>> Handle(CreateRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateRegistrationPeriodDtoValidator(_unitOfWork,
                request.CreateRegistrationPeriodDto.TimeStart ?? DateTime.Now);
            var validationResult = await validator.ValidateAsync(request.CreateRegistrationPeriodDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<RegistrationPeriodDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var period = _mapper.Map<RegistrationPeriod>(request.CreateRegistrationPeriodDto);

                var curent = await _registrationPeriodRepo.GetCurrentRegistrationPeriodAsync(
                    new CurrentRegistrationPeriodRequest() {
                        schoolYear = period.SchoolYear,
                        semester = period.Semester,
                        facultyId = period.FacultyId,
                    });
                period.Phase = curent == null ? 1 : curent.Phase + 1;
                
                if(period.TimeStart.Value.Month >= 8)
                {
                    period.SchoolYear = (period.TimeStart.Value.Year) + "-" + (period.TimeStart.Value.Year + 1);
                }    
                else
                {
                    period.SchoolYear = (period.TimeStart.Value.Year - 1) + "-" + (period.TimeStart.Value.Year);
                }    

                var newRegistrationPeriod = await _unitOfWork.Repository<RegistrationPeriod>()
                                                             .AddAsync(period);
                await _unitOfWork.Save(cancellationToken);

                var periodDto = _mapper.Map<RegistrationPeriodDto>(newRegistrationPeriod);

                return Result<RegistrationPeriodDto>.Success(periodDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
