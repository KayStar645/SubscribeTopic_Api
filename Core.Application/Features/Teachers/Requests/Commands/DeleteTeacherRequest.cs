using MediatR;

namespace Core.Application.Features.Teachers.Requests.Commands
{
    public class DeleteTeacherRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
