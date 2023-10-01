using MediatR;

namespace Core.Application.Features.FacultyDuties.Requests.Commands
{
    public class DeleteFacultyDutyRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
