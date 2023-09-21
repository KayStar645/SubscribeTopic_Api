using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Majors.Requests.Queries
{
    public class ListMajorRequest<T> : ListBaseRequest<T>
    {
        public bool isGetFaculty { get; set; }
    }
}
