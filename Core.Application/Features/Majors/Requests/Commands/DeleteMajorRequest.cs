using MediatR;

namespace Core.Application.Features.Majors.Requests.Commands
{
    public class DeleteMajorRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
