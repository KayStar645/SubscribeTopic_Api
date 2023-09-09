using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Faculties.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Faculties.Handlers.Commands
{
    public class DeleteFacultyCommandHandler : IRequestHandler<DeleteFacultyRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteFacultyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteFacultyRequest request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.Repository<Faculty>().GetByIdAsync(request.Id);

            if (faculty == null)
                throw new NotFoundException(nameof(Faculty), request.Id);

            await _unitOfWork.Repository<Faculty>().DeleteAsync(faculty);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
