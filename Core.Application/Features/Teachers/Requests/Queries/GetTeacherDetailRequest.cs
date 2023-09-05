using Core.Application.DTOs.Teacher;
using MediatR;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class GetTeacherDetailRequest : IRequest<TeacherDto>
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
