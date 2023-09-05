using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.DTOs.Teacher.Validators;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseCommandResponse> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new CreateTeacherDtoValidator();
            var validationResult = await validator.ValidateAsync(request.TeacherDto);

            if (validationResult.IsValid == false)
            {
                response.Success = false;
                response.Message = CustomConstant.ACTION_FAILED_CREATE;
                response.Errors = validationResult.Errors.Select(q => q.ErrorMessage).ToList();
            }    
            else
            {
                var teacher = _mapper.Map<Teacher>(request.TeacherDto);

                teacher = await _unitOfWork.Repository<Teacher>().AddAsync(teacher);
                await _unitOfWork.Save(cancellationToken);

                response.Success = true;
                response.Message = CustomConstant.ACTION_SUCCESS_CREATE;
                response.Id = teacher.Id;
            }

            return response;
        }
    }
}
