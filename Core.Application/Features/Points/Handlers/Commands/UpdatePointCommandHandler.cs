using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
using Core.Application.DTOs.Point.Validators;
using Core.Application.Features.Points.Events;
using Core.Application.Features.Points.Requests.Commands;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Points.Handlers.Commands
{
    public class UpdatePointCommandHandler : IRequestHandler<UpdatePointRequest, Result<PointDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public UpdatePointCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<PointDto>> Handle(UpdatePointRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdatePointDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.updatePointDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<PointDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var point = _mapper.Map<Point>(request.updatePointDto);
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeUpdatePointUpdatePointEvent(point, httpContextAccessor, unitOfWork));
                    }
                });

                // Kiểm tra giảng viên chấm cho sv tham gia này chưa
                var findPoint = await _unitOfWork.Repository<Point>()
                            .FirstOrDefaultAsync(x => x.StudentJoinId == point.StudentJoinId &&
                                                      x.TeacherId == point.TeacherId);
                if (findPoint == null)
                {
                    return Result<PointDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updatePointDto.StudentJoinId.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findPoint.Scores = point.Scores;
                var newPoint = await _unitOfWork.Repository<Point>().UpdateAsync(findPoint);
                await _unitOfWork.Save(cancellationToken);

                var pointDto = _mapper.Map<PointDto>(newPoint);

                return Result<PointDto>.Success(pointDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<PointDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}