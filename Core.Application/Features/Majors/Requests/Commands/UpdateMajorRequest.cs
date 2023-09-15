using Core.Application.DTOs.Major;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Majors.Requests.Commands
{
    public class UpdateMajorRequest : IRequest<Result<MajorDto>>
    {
        public UpdateMajorDto? UpdateMajorDto { get; set; }
    }
}
