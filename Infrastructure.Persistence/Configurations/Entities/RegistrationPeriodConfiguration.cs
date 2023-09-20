using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Entities
{
    public class RegistrationPeriodConfiguration : IEntityTypeConfiguration<RegistrationPeriod>
    {
        public void Configure(EntityTypeBuilder<RegistrationPeriod> builder)
        {
            builder
                .HasIndex(x => new { x.SchoolYear, x.Semester, x.FacultyId })
                .HasName("IX_SchoolYear_Semester_FacultyId");

            builder
                .HasIndex(x => new { x.SchoolYear, x.Semester, x.Phase, x.FacultyId })
                .HasName("IX_SchoolYear_Semester_Phase_FacultyId");
        }
    }
}
