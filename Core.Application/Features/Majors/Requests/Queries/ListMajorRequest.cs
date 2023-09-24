using Core.Application.DTOs.Major;
using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Majors.Requests.Queries
{
    public class ListMajorRequest : ListBaseRequest<MajorDto>
    {
        public bool isGetIndustry { get; set; }
    }
}
