using AutoMapper;
using FluentValidation.Results;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Teachers.Requests.Commands;
using MediatR;
using Core.Domain.Entities;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateTeacherDtoValidator();
            ValidationResult validationResult = await validator.ValidateAsync(request.UpdateTeacherDto);

            if (validationResult.IsValid == false)
                throw new ValidationException(validationResult);

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.UpdateTeacherDto.Id);

            if (teacher is null)
                throw new NotFoundException(nameof(teacher), request.UpdateTeacherDto.Id);

            _mapper.Map(request.UpdateTeacherDto, teacher);

            await _unitOfWork.Repository<Teacher>().UpdateAsync(teacher);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
