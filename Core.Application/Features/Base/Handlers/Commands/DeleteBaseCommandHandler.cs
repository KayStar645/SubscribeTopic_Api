using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Domain.Common;
using MediatR;

namespace Core.Application.Features.Base.Handlers.Commands
{
    public class DeleteBaseCommandHandler<T> : IRequestHandler<DeleteBaseRequest<T>, Unit>
        where T : BaseAuditableEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteBaseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteBaseRequest<T> request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(T), request.Id);

            await _unitOfWork.Repository<T>().DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
