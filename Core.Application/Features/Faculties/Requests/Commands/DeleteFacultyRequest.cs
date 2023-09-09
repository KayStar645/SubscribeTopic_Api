using MediatR;

namespace Core.Application.Features.Faculties.Requests.Commands
{
    public class DeleteFacultyRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
