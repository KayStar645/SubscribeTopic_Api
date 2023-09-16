using Core.Application.DTOs.Teacher;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class DetailTeacherRequest : DetailBaseRequest, IRequest<Result<TeacherDto>>
    {
        public bool IsGetDepartment { get; set; }
        public string? Type { get; set; } = Teacher.TYPE_TEACHER_LECTURERS;
    }
}
