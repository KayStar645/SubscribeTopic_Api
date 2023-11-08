namespace Core.Application.DTOs.Thesis
{
    public interface IThesisDto
    {
        public int? ParentId { get; set; }

        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

    }
}
