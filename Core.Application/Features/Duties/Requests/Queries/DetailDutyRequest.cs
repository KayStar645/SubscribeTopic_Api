using Core.Application.DTOs.Duty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Queries
{
    public class DetailDutyRequest : DetailBaseRequest, IRequest<Result<DutyDto>>
    {
        public bool? isGetFaculty { get; set; }

        public bool? isGetDepartment { get; set; }

        public bool? isGetTeacher { get; set; }
    }
}
