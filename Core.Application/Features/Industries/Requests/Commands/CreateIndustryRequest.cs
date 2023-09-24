using Core.Application.DTOs.Industry;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Industries.Requests.Commands
{
    public class CreateIndustryRequest : IRequest<Result<IndustryDto>>
    {
        public CreateIndustryDto createIndustryDto { get; set; }
    }
}
