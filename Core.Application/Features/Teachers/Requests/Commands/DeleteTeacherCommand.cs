using MediatR;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class DeleteTeacherCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
