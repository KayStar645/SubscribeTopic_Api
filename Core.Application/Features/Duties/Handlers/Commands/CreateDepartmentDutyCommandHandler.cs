using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.DTOs.Duty.Validators;
using Core.Application.Features.Duties.Events;
using Core.Application.Features.Duties.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using DutyEntity = Core.Domain.Entities.Duty;

namespace Core.Application.Features.Duties.Handlers.Commands
{
    public class CreateDepartmentDutyCommandHandler : IRequestHandler<CreateDepartmentDutyRequest, Result<DepartmentDutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateDepartmentDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<DepartmentDutyDto>> Handle(CreateDepartmentDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateDepartmentDutyDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createDepartmentDutyDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DepartmentDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var dutyFaculty = await _unitOfWork.Repository<DutyEntity>()
                        .FirstOrDefaultAsync(x => x.Type == DutyEntity.TYPE_FACULTY &&
                                        x.Id == request.createDepartmentDutyDto.DutyId);
                if(request.createDepartmentDutyDto.TimeEnd >= dutyFaculty.TimeEnd)
                {
                    return Result<DepartmentDutyDto>
                        .Failure(ValidatorTransform.LessThanDay("timeEnd", (DateTime)dutyFaculty.TimeEnd),
                        (int)HttpStatusCode.BadRequest);
                }    

                var duty = _mapper.Map<Duty>(request.createDepartmentDutyDto);
                duty.Type = Duty.TYPE_DEPARTMENT;

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateDepartmentDutyUpdateDutyEvent(duty, httpContextAccessor, unitOfWork));

                    }
                });

                var newDuty = await _unitOfWork.Repository<Duty>().AddAsync(duty);
                await _unitOfWork.Save(cancellationToken);

                var departmentDutyDto = _mapper.Map<DepartmentDutyDto>(newDuty);

                return Result<DepartmentDutyDto>.Success(departmentDutyDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
