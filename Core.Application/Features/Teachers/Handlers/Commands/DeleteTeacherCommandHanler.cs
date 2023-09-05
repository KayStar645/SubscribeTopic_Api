using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class DeleteTeacherCommandHanler : IRequestHandler<DeleteTeacherCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteTeacherCommandHanler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.Id);

            if (teacher is null)
                throw new NotFoundException(nameof(Teacher), request.Id);
            
            teacher.IsDeleted = true;
            await _unitOfWork.Repository<Teacher>().DeleteAsync(teacher);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
