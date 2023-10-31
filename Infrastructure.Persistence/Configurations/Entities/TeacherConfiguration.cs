using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Entities
{
    public class TeacherConfiguration: IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasOne(x => x.Dean_Faculty)
                .WithOne(x => x.Dean_Teacher)
                .HasForeignKey<Faculties>(f => f.Dean_TeacherId);

            builder.HasOne(x => x.HeadDepartment_Department)
                .WithOne(x => x.HeadDepartment_Teacher)
                .HasForeignKey<Department>(d => d.HeadDepartment_TeacherId);

            builder.Property(b => b.Type).HasDefaultValue(Teacher.TYPE_TEACHER_LECTURERS);
        }
    }
}
