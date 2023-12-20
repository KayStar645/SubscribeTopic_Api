using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Commissioner;
using Core.Application.DTOs.Council;
using Core.Application.DTOs.Council.Validators;
using Core.Application.Features.Councils.Events;
using Core.Application.Features.Councils.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Councils.Handlers.Commands
{
    public class CreateCouncilCommandHandler : IRequestHandler<CreateCouncilRequest, Result<CouncilDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateCouncilCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<CouncilDto>> Handle(CreateCouncilRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateCouncilDtoValidator(_unitOfWork);
            var validatorResult = await validator.ValidateAsync(request.createCouncilDto);

            if (validatorResult.IsValid == false)
            {
                var errorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<CouncilDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var council = _mapper.Map<Council>(request.createCouncilDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateCouncilUpdateCouncilEvent(council, httpContextAccessor, unitOfWork));

                    }
                });

                var newCouncil = await _unitOfWork.Repository<Council>().AddAsync(council);
                await _unitOfWork.Save(cancellationToken);

                var councilDto = _mapper.Map<CouncilDto>(newCouncil);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterCreateCouncilCreateCommissionersEvent(request.createCouncilDto, councilDto, unitOfWork));

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
                return Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
