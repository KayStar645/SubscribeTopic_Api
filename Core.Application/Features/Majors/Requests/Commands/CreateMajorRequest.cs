using Core.Application.DTOs.Major;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Majors.Requests.Commands
{
    public class CreateMajorRequest : IRequest<Result<MajorDto>>
    {
        public CreateMajorDto? createMajorDto { get; set; }
    }
}
