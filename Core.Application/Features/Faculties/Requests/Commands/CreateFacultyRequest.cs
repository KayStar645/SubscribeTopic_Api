using Core.Application.DTOs.Faculty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Faculties.Requests.Commands
{
    public class CreateFacultyRequest : IRequest<Result<FacultyDto>>
    {
        public CreateFacultyDto? createFacultyDto { get; set; }
    }
}
