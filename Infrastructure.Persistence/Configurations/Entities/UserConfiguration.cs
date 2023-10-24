using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Entities
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(x => x.Teacher)
                .WithOne(x => x.User)
                .HasForeignKey<Teacher>(f => f.UserId);

            builder.HasOne(x => x.Student)
                .WithOne(x => x.User)
                .HasForeignKey<Student>(f => f.UserId);
        }
    }
}
