using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Student.Validators;
using Core.Application.Features.Students.Events;
using Core.Application.Features.Students.Requests.Commands;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Students.Handlers.Commands
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentRequest, Result<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<StudentDto>> Handle(CreateStudentRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateStudentDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createStudentDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var student = _mapper.Map<Student>(request.createStudentDto);

                var newStudent = await _unitOfWork.Repository<Student>().AddAsync(student);
                await _unitOfWork.Save(cancellationToken);

                var studentDto = _mapper.Map<StudentDto>(newStudent);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        await mediator.Publish(new AfterCreateStudentCreateAccountEvent(newStudent, unitOfWork, userRepository));
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<StudentDto>.Success(studentDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}