using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Teacher.Validators;
using Core.Application.Features.Teachers.Events;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherRequest, Result<TeacherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<TeacherDto>> Handle(CreateTeacherRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateTeacherDtoValidator(_unitOfWork, request.createTeacherDto.DepartmentId);
            var validationResult = await validator.ValidateAsync(request.createTeacherDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<TeacherDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var teacher = _mapper.Map<Teacher>(request.createTeacherDto);

                var newTeacher = await _unitOfWork.Repository<Teacher>().AddAsync(teacher);
                await _unitOfWork.Save(cancellationToken);

                var teacherDto = _mapper.Map<TeacherDto>(newTeacher);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        await mediator.Publish(new AfterCreateTeacherCreateAccountEvent(newTeacher, unitOfWork, userRepository));
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<TeacherDto>.Success(teacherDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
