using Core.Application.DTOs.StudentJoin;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.StudentJoins.Requests.Commands
{
    public class CreateStudentJoinRequest : IRequest<Result<List<StudentJoinDto>>>
    {
        public CreateStudentJoinDto createStudentJoinDto { get; set; }
    }
}
