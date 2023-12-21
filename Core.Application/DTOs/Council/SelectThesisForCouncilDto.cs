namespace Core.Application.DTOs.Council
{
    public class SelectThesisForCouncilDto
    {
        public int? CouncilId { get; set; }

        public List<ThesisScheduleDto>? ListThesis { get; set; }
    }
}
