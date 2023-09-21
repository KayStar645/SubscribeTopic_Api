using Core.Application.DTOs.Teacher;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class DetailTeacherRequest : DetailBaseRequest, IRequest<Result<TeacherDto>>
    {
        public bool isGetDepartment { get; set; }
        public string? type { get; set; } = Teacher.TYPE_TEACHER_LECTURERS;
    }
}
