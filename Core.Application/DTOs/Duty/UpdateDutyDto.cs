using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Duty
{
    public class UpdateDutyDto: BaseDto, IDutyDto
    {
        public string? Name { get; set; }

        public string? Content { get; set; }

        public int? NumberOfThesis { get; set; }

        public DateTime? TimeEnd { get; set; }

        public List<string>? Files { get; set; }
    }
}
