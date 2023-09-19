using MediatR;

namespace Core.Application.Features.Base.Requests.Commands
{
    public class DeleteBaseRequest<T> : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
