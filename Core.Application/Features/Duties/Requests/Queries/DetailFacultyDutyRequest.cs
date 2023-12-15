using Core.Application.DTOs.Duty;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Queries
{
    public class DetailFacultyDutyRequest : DetailBaseRequest, IRequest<Result<FacultyDutyDto>>
    {
        public bool? isGetFaculty { get; set; }

        public bool? isGetDepartment { get; set; }
    }
}
