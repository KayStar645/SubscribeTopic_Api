using Core.Application.DTOs.Student;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Students.Requests.Commands
{
    public class CreateStudentRequest : IRequest<Result<StudentDto>>
    {
        public CreateStudentDto? createStudentDto { get; set; }
    }
}
