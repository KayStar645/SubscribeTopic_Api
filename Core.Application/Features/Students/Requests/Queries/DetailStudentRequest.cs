using Core.Application.DTOs.Student;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Students.Requests.Queries
{
    public class DetailStudentRequest : DetailBaseRequest, IRequest<Result<StudentDto>>
    {
        public bool IsGetMajor { get; set; }
    }
}
