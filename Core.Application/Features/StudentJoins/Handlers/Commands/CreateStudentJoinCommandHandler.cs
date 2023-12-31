﻿using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.StudentJoin.Validators;
using Core.Application.Features.StudentJoins.Events;
using Core.Application.Features.StudentJoins.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.StudentJoins.Handlers.Commands
{
    public class CreateStudentJoinCommandHandler : IRequestHandler<CreateStudentJoinRequest, Result<List<StudentJoinDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateStudentJoinCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<List<StudentJoinDto>>> Handle(CreateStudentJoinRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateStudentJoinDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createStudentJoinDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<List<StudentJoinDto>>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                List<StudentJoinDto> list = new List<StudentJoinDto>();
                foreach (var studentsId in request.createStudentJoinDto.studentIds)
                {
                    var studentJoin = new StudentJoin
                    {
                        StudentId = studentsId,
                        RegistrationPeriodId = request.createStudentJoinDto.registrationPeriodId
                    };

                    var findStudentJoin = await _unitOfWork.Repository<StudentJoin>()
                                .FirstOrDefaultAsync(x => x.StudentId == studentJoin.StudentId &&
                                        x.RegistrationPeriodId == studentJoin.RegistrationPeriodId);
                    var studentJoinDto = new StudentJoinDto();
                    if (findStudentJoin == null)
                    {
                        var newStudentJoin = await _unitOfWork.Repository<StudentJoin>().AddAsync(studentJoin);
                        await _unitOfWork.Save(cancellationToken);

                        studentJoinDto = _mapper.Map<StudentJoinDto>(newStudentJoin);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        await Task.Run(async () =>
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                                await mediator.Publish(new AfterCreatedStudentJoinCreateGroupEvent(newStudentJoin, unitOfWork));
                            }
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                    else
                    {
                        studentJoinDto = _mapper.Map<StudentJoinDto>(findStudentJoin);
                    }    


                    list.Add(studentJoinDto);
                }
                return Result<List<StudentJoinDto>>.Success(list, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<List<StudentJoinDto>>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
