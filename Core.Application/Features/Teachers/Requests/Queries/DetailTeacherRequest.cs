using Core.Application.DTOs.Teacher;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class DetailTeacherRequest : IRequest<Result<TeacherDto>>
    {
        public int Id { get; set; }
    }
}
