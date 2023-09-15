using MediatR;

namespace Core.Application.Features.Students.Requests.Commands
{
    public class DeleteStudentRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
