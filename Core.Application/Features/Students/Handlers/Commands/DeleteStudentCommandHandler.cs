using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Students.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Students.Handlers.Commands
{
    public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(DeleteStudentRequest request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.Id);

            if (student is null)
            {
                throw new NotFoundException(nameof(Student), request.Id);
            }

            await _unitOfWork.Repository<Student>().DeleteAsync(student);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
