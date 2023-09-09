using Core.Application.DTOs.Teacher;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class UpdateTeacherRequest : IRequest<Result<TeacherDto>>
    {
        public UpdateTeacherDto? UpdateTeacherDto { get; set; }
    }
}
