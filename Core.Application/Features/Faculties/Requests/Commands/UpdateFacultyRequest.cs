using Core.Application.DTOs.Faculty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Faculties.Requests.Commands
{
    public class UpdateFacultyRequest : IRequest<Result<FacultyDto>>
    {
        public UpdateFacultyDto? updateFacultyDto { get; set; }
    }
}
