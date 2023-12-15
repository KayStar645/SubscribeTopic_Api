using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
using Core.Application.DTOs.Point.Validators;
using Core.Application.Features.Points.Events;
using Core.Application.Features.Points.Requests.Commands;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Points.Handlers.Commands
{
    public class CreateOrUpdatePointCommandHandler : IRequestHandler<CreateOrUpdatePointRequest, Result<PointDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateOrUpdatePointCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<PointDto>> Handle(CreateOrUpdatePointRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateOrUpdatePointDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createOrUpdatePointDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<PointDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var point = _mapper.Map<Point>(request.createOrUpdatePointDto);
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateOrUpdatePointUpdatePointEvent(point, httpContextAccessor, unitOfWork));
                    }
                });

                // Kiểm tra giảng viên chấm cho sv tham gia này chưa
                var checkPoint = await _unitOfWork.Repository<Point>()
                            .FirstOrDefaultAsync(x => x.StudentJoinId == point.StudentJoinId &&
                                                      x.TeacherId == point.TeacherId);
                if (checkPoint == null)
                {
                    checkPoint = await _unitOfWork.Repository<Point>().AddAsync(point);
                    await _unitOfWork.Save(cancellationToken);
                }
                else
                {
                    checkPoint.Scores = point.Scores;
                    checkPoint = await _unitOfWork.Repository<Point>().UpdateAsync(checkPoint);
                    await _unitOfWork.Save(cancellationToken);
                }    

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        await mediator.Publish(new AfterCreateOrUpdatePointUpdateStudentJoinEvent(checkPoint, unitOfWork, userRepository));
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                var pointDto = _mapper.Map<PointDto>(checkPoint);

                return Result<PointDto>.Success(pointDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<PointDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}