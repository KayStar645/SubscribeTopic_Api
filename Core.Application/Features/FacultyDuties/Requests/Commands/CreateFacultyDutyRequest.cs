using Core.Application.DTOs.FacultyDuty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.FacultyDuties.Requests.Commands
{
    public class CreateFacultyDutyRequest : IRequest<Result<FacultyDutyDto>>
    {
        public CreateFacultyDutyDto? CreateFacultyDutyDto { get; set; }
    }
}
