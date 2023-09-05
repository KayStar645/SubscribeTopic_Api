using Core.Application.DTOs.Teacher;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class UpdateTeacherCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public UpdateTeacherDto? UpdateTeacherDto { get; set; }
    }
}
