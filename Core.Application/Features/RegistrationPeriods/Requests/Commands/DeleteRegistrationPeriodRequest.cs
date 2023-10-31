using MediatR;

namespace Core.Application.Features.RegistrationPeriods.Requests.Commands
{
    public class DeleteRegistrationPeriodRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
