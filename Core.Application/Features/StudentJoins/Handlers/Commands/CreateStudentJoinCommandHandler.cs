using AutoMapper;
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
    public class CreateStudentJoinCommandHandler : IRequestHandler<CreateStudentJoinRequest, Result<StudentJoinDto>>
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

        public async Task<Result<StudentJoinDto>> Handle(CreateStudentJoinRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateStudentJoinDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createStudentJoinDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentJoinDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var studentJoin = _mapper.Map<StudentJoin>(request.createStudentJoinDto);

                var newStudentJoin = await _unitOfWork.Repository<StudentJoin>().AddAsync(studentJoin);
                await _unitOfWork.Save(cancellationToken);

                var studentJoinDto = _mapper.Map<StudentJoinDto>(newStudentJoin);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new CreateGroupAfterCreatedStudentJoinEvent(newStudentJoin, unitOfWork));
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<StudentJoinDto>.Success(studentJoinDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
