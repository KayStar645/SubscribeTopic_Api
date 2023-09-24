using Core.Application.DTOs.Industry;
using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Industries.Requests.Queries
{
    public class ListIndustryRequest : ListBaseRequest<IndustryDto>
    {
        public bool isGetFaculty { get; set; }
    }
}
