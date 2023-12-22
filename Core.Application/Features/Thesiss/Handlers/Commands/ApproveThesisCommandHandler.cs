using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Events;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Commands
{
    public class ApproveThesisCommandHandler : IRequestHandler<ApproveThesisRequest, Result<ThesisDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public ApproveThesisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<ThesisDto>> Handle(ApproveThesisRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var findThesis = await _unitOfWork.Repository<Thesis>().GetByIdAsync(request.approveThesisDto.Id);

                if (findThesis is null)
                {
                    return Result<ThesisDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.approveThesisDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }
                if(findThesis.Status != Thesis.STATUS_APPROVE_REQUEST)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }    

                findThesis.Status = Thesis.STATUS_APPROVED;
                var newThesis = await _unitOfWork.Repository<Thesis>().UpdateAsync(findThesis);
                await _unitOfWork.Save(cancellationToken);

                var thesisDto = _mapper.Map<ThesisDto>(newThesis);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterApproveThesisUpdateDutyEvent(newThesis, unitOfWork));

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (NotFoundException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
