using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class DeleteTeacherCommandHanler : IRequestHandler<DeleteTeacherRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteTeacherCommandHanler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteTeacherRequest request, CancellationToken cancellationToken)
        {
            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.Id);

            if (teacher is null)
            {
                throw new NotFoundException(nameof(Teacher), request.Id);
            }

            await _unitOfWork.Repository<Teacher>().DeleteAsync(teacher);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
