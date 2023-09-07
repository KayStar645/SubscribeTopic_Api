using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Departments.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Departments.Handlers.Commands
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Repository<Department>().GetByIdAsync(request.Id);

            if (department == null)
                throw new NotFoundException(nameof(Department), request.Id);

            await _unitOfWork.Repository<Department>().DeleteAsync(department);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
