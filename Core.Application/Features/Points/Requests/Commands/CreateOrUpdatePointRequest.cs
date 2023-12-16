using Core.Application.DTOs.Point;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Points.Requests.Commands
{
    public class CreateOrUpdatePointRequest : IRequest<Result<PointDto>>
    {
        public CreateOrUpdatePointDto? createOrUpdatePointDto { get; set; }
    }
}
