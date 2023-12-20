using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Commissioner;
using Core.Application.DTOs.Council;
using Core.Application.DTOs.Council.Validators;
using Core.Application.Features.Councils.Events;
using Core.Application.Features.Councils.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Councils.Handlers.Commands
{
    public class UpdateCouncilCommandHandler : IRequestHandler<UpdateCouncilRequest, Result<CouncilDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public UpdateCouncilCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<CouncilDto>> Handle(UpdateCouncilRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateCouncilDtoValidator(_unitOfWork, request.updateCouncilDto.Id);
            var errorValidator = await validator.ValidateAsync(request.updateCouncilDto);

            if (errorValidator.IsValid == false)
            {
                var errorMessage = errorValidator.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<CouncilDto>.Failure(errorMessage, (int)HttpStatusCode.BadRequest);
            }


            try
            {
                var findCouncil = await _unitOfWork.Repository<Council>().GetByIdAsync(request.updateCouncilDto.Id);

                if (findCouncil is null)
                {
                    return Result<CouncilDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateCouncilDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findCouncil.CopyPropertiesFrom(request.updateCouncilDto);

                var newCouncil = await _unitOfWork.Repository<Council>().UpdateAsync(findCouncil);
                await _unitOfWork.Save(cancellationToken);

                var councilDto = _mapper.Map<CouncilDto>(newCouncil);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterUpdateCouncilCreateOrUpdateCommissionersEvent(request.updateCouncilDto, councilDto, unitOfWork));

                    }
                });
                var com = await _unitOfWork.Repository<Commissioner>()
                            .Query().Where(x => x.CouncilId == councilDto.Id)
                            .Include(x => x.Teacher)
                            .ToListAsync();
                councilDto.Commissioners = _mapper.Map<List<CommissionerDto>>(com);

                return Result<CouncilDto>.Success(councilDto, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
