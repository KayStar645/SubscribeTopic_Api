using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Entities
{
    public class StudentJoinConfiguration : IEntityTypeConfiguration<StudentJoin>
    {
        public void Configure(EntityTypeBuilder<StudentJoin> builder)
        {
            
        }
    }
}
