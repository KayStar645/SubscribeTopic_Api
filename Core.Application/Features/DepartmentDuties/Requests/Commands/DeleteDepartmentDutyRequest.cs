using MediatR;

namespace Core.Application.Features.DepartmentDuties.Requests.Commands
{
    public class DeleteDepartmentDutyRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
