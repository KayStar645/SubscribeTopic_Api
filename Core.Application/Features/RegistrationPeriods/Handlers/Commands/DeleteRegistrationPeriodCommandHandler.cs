using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Commands
{
    public class DeleteRegistrationPeriodCommandHandler : IRequestHandler<DeleteRegistrationPeriodRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            var period = await _unitOfWork.Repository<RegistrationPeriod>().GetByIdAsync(request.Id);

            if (period is null)
            {
                throw new NotFoundException(nameof(RegistrationPeriod), request.Id);
            }

            await _unitOfWork.Repository<RegistrationPeriod>().DeleteAsync(period);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
