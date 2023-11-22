using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Entities
{
    public class ThesisRegistrationConfignuration : IEntityTypeConfiguration<ThesisRegistration>
    {
        public void Configure(EntityTypeBuilder<ThesisRegistration> builder)
        {
            builder.HasOne(x => x.Group)
                .WithOne(x => x.ThesisRegistration)
                .HasForeignKey<ThesisRegistration>(x => x.GroupId);

            builder.HasOne(x => x.Thesis)
                .WithOne(x => x.ThesisRegistration)
                .HasForeignKey<ThesisRegistration>(x => x.ThesisId);
        }
    }
}
