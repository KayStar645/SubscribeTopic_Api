using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Commands
{
    public class CreateFacultyDutyRequest : IRequest<Result<FacultyDutyDto>>
    {
        public CreateFacultyDutyDto? createFacultyDutyDto { get; set; }
    }
}
