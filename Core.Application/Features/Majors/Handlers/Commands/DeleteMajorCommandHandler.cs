using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Majors.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Majors.Handlers.Commands
{
    public class DeleteMajorCommandHandler : IRequestHandler<DeleteMajorRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteMajorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(DeleteMajorRequest request, CancellationToken cancellationToken)
        {
            var major = await _unitOfWork.Repository<Major>().GetByIdAsync(request.Id);

            if (major is null)
            {
                throw new NotFoundException(nameof(Major), request.Id);
            }

            await _unitOfWork.Repository<Major>().DeleteAsync(major);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
