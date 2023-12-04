namespace Core.Application.Models.GoogleDrive
{
    public class UploadResponse
    {
        public string? Name { get; set; }
        public string? Path { get; set; }

        public string? Type { get; set; }

        public long? SizeInBytes { get; set; }
    }
}
