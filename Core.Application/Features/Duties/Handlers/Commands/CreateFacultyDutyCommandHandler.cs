﻿using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.DTOs.Duty.Validators;
using Core.Application.Features.Duties.Events;
using Core.Application.Features.Duties.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Commands
{
    public class CreateFacultyDutyCommandHandler : IRequestHandler<CreateFacultyDutyRequest, Result<FacultyDutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateFacultyDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<FacultyDutyDto>> Handle(CreateFacultyDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateFacultyDutyDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createFacultyDutyDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var duty = _mapper.Map<Duty>(request.createFacultyDutyDto);
                duty.Type = Duty.TYPE_FACULTY;

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateFacultyDutyUpdateDutyEvent(duty, httpContextAccessor, unitOfWork));

                    }
                });

                var newDuty = await _unitOfWork.Repository<Duty>().AddAsync(duty);
                await _unitOfWork.Save(cancellationToken);

                var FacultyDutyDto = _mapper.Map<FacultyDutyDto>(newDuty);

                return Result<FacultyDutyDto>.Success(FacultyDutyDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
