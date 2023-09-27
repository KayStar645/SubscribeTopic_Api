using Core.Application.DTOs.FacultyDuty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.FacultyDuties.Requests.Commands
{
    public class UpdateFacultyDutyRequest : IRequest<Result<FacultyDutyDto>>
    {
        public UpdateFacultyDutyDto? UpdateFacultyDutyDto { get; set; }
    }
}
