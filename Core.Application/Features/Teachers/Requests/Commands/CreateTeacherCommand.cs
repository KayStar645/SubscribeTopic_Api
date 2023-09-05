using Core.Application.DTOs.Teacher;
using MediatR;
using Shared;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class CreateTeacherCommand : IRequest<Result<TeacherDto>>
    {
        public CreateTeacherDto? TeacherDto { get; set; }
    }
}
