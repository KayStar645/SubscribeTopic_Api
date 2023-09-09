using MediatR;

namespace Core.Application.Features.Departments.Requests.Commands
{
    public class DeleteDepartmentRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
