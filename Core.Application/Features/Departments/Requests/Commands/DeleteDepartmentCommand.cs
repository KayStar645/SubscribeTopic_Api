using MediatR;

namespace Core.Application.Features.Departments.Requests.Commands
{
    public class DeleteDepartmentCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
