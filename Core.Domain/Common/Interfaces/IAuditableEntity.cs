namespace Core.Domain.Common.Interfaces
{
    public interface IAuditableEntity : IEntity
    {
        DateTime DateCreated { get; set; }
        string CreatedBy { get; set; }
        DateTime LastModifiedDate { get; set; }
        string LastModifiedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
