using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.Thesis.Validators;
using Core.Application.Features.Thesiss.Events;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Commands
{
    public class UpdateThesisCommandHandler : IRequestHandler<UpdateThesisRequest, Result<ThesisDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public UpdateThesisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<ThesisDto>> Handle(UpdateThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateThesisDtoValidator(_unitOfWork, request.updateThesisDto.MinQuantity, request.updateThesisDto.Id);
            var validationResult = await validator.ValidateAsync(request.updateThesisDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findThesis = await _unitOfWork.Repository<Thesis>().GetByIdAsync(request.updateThesisDto.Id);

                if (findThesis is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateThesisDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findThesis.CopyPropertiesFrom(request.updateThesisDto);

                var newThesis = await _unitOfWork.Repository<Thesis>().UpdateAsync(findThesis);
                await _unitOfWork.Save(cancellationToken);

                var thesisDto = _mapper.Map<ThesisDto>(newThesis);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent(request.updateThesisDto, thesisDto, unitOfWork));
                        await mediator.Publish(new AfterUpdateThesisCreateOrUpdateThesisReviewsEvent(request.updateThesisDto, thesisDto, unitOfWork));
                        await mediator.Publish(new AfterUpdateThesisCreateOrUpdateThesisMajorsEvent(request.updateThesisDto, thesisDto, unitOfWork));

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
