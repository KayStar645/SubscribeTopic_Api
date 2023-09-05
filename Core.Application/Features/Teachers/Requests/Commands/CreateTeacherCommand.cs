using Core.Application.DTOs.Teacher;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class CreateTeacherCommand : IRequest<BaseCommandResponse>
    {
        public CreateTeacherDto? TeacherDto { get; set; }
    }
}
