using Core.Application.DTOs.Thesis;
using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Thesiss.Requests.Queries
{
    public class ListThesisReviewOfTeacherRequest : ListBaseRequest<ThesisDto>
    {
        public bool? isGetIssuer { get; set; }

        public bool? isGetThesisInstructions { get; set; }

        public bool? isGetThesisReviews { get; set; }

        public bool? isGetThesisMajors { get; set; }

        public bool? isGetAll { get; set; }
    }
}
