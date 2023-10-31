using Core.Application.DTOs.StudentJoin;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.StudentJoins.Requests.Queries
{
    public class DetailStudentJoinRequest : DetailBaseRequest, IRequest<Result<StudentJoinDto>>
    {
        public bool isGetStudent { get; set; }
        public bool isGetRegistrationPeriod { get; set; }
    }
}
