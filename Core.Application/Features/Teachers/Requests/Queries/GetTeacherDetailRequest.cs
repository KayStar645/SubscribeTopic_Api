using Core.Application.DTOs.Teacher;
using MediatR;
using Shared;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class GetTeacherDetailRequest : IRequest<Result<TeacherDto>>
    {
        public int Id { get; set; }
    }
}
