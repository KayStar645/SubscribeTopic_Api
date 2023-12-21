using Core.Application.DTOs.Thesis;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Queries
{
    public class DetailThesisMePeriodCurrentRequest : DetailBaseRequest, IRequest<Result<ThesisDto>>
    {


    }
}
