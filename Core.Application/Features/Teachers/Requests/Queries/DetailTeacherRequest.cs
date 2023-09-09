using Core.Application.DTOs.Teacher;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class DetailTeacherRequest : DetailBaseRequest, IRequest<Result<TeacherDto>>
    {
        public bool IsGetDepartment { get; set; }
    }
}
