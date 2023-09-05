using Core.Application.DTOs.Teacher;
using MediatR;
using Shared;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class UpdateTeacherCommand : IRequest<Result<TeacherDto>>
    {
        public int Id { get; set; }

        public UpdateTeacherDto? UpdateTeacherDto { get; set; }
    }
}
