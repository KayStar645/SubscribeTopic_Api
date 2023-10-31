using Core.Application.DTOs.Industry;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Industries.Requests.Commands
{
    public class UpdateIndustryRequest : IRequest<Result<IndustryDto>>
    {
        public UpdateIndustryDto updateIndustryDto { get; set; }
    }
}
