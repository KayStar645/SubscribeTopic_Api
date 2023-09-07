namespace Core.Application.DTOs.Department
{
    public class CreateDepartmentDto : IDepartmentDto
    {
        public string InternalCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
