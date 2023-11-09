namespace Core.Application.DTOs.Thesis
{
    public class CreateThesisDto : IThesisDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public string? Summary { get; set; }

        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        // Giảng viên hướng dẫn
        public List<int?>? ThesisInstructionsId { get; set; }

        // Chuyên ngành phù hợp
        public List<int?>? ThesisMajorsId { get; set; }
    }
}
