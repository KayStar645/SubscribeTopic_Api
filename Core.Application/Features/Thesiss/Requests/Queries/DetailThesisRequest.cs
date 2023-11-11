using Core.Application.DTOs.Thesis;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Queries
{
    public class DetailThesisRequest : DetailBaseRequest, IRequest<Result<ThesisDto>>
    {
        public bool? isGetIssuer { get; set; }

        public bool? isGetThesisInstructions { get; set; }

        public bool? isGetThesisReviews { get; set; }

        public bool? isGetThesisMajors { get; set; }

    }
}
