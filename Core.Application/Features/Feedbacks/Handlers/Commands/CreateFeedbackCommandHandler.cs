using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Feedback;
using Core.Application.DTOs.Feedback.Validator;
using Core.Application.Exceptions;
using Core.Application.Features.Feedbacks.Events;
using Core.Application.Features.Feedbacks.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Feedbacks.Handlers.Commands
{
    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackRequest, Result<FeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateFeedbackCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<FeedbackDto>> Handle(CreateFeedbackRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateFeedbackDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createFeedbackDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FeedbackDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var feedback = _mapper.Map<Feedback>(request.createFeedbackDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateFeedbackUpdateFeedbackEvent(feedback, httpContextAccessor, unitOfWork));
                    }
                });

                var newFeedback = await _unitOfWork.Repository<Feedback>().AddAsync(feedback);
                await _unitOfWork.Save(cancellationToken);

                var feedbackDto = _mapper.Map<FeedbackDto>(newFeedback);

                return Result<FeedbackDto>.Success(feedbackDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<FeedbackDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<FeedbackDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<FeedbackDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<FeedbackDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
