using Core.Application.DTOs.Point;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Points.Requests.Commands
{
    public class UpdatePointRequest : IRequest<Result<PointDto>>
    {
        public UpdatePointDto? updatePointDto { get; set; }
    }
}
