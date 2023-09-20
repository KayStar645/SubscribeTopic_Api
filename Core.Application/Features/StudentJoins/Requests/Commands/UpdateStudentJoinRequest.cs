using Core.Application.DTOs.StudentJoin;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.StudentJoins.Requests.Commands
{
    public class UpdateStudentJoinRequest : IRequest<Result<StudentJoinDto>>
    {
        public UpdateStudentJoinDto updateStudentJoinDto { get; set; }
    }
}
