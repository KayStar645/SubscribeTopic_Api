using Core.Application.DTOs.Exchanges;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Exchanges.Requests.Commands
{
    public class CreateExchangeRequest : IRequest<Result<ExchangeDto>>
    {
        public CreateExchangeDto? createExchangeDto { get; set; }
    }
}
