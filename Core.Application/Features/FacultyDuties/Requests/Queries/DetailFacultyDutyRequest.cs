using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.FacultyDuties.Requests.Queries
{
    public class DetailFacultyDutyRequest : DetailBaseRequest, IRequest<Result<FacultyDutyDto>>
    {
        public bool isGetFaculty { get; set; }
        public bool isGetDepartment { get; set; }
    }
}
