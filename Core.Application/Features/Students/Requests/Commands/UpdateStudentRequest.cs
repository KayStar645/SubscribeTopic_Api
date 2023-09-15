using Core.Application.DTOs.Student;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Students.Requests.Commands
{
    public class UpdateStudentRequest : IRequest<Result<StudentDto>>
    {
        public UpdateStudentDto? UpdateStudentDto { get; set; }
    }
}
