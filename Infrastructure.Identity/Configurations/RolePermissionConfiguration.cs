using Core.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissions>
    {
        public void Configure(EntityTypeBuilder<RolePermissions> builder)
        {
            builder.ToTable(nameof(RolePermissions));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.RoleId).IsRequired();
            builder.Property(x => x.PermissionId).IsRequired();

            builder.HasOne(x => x.Roles)
                .WithMany(t => t.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .HasConstraintName("FK_RolePermission_Role_RoleId");

            builder.HasOne(x => x.Permissions)
                .WithMany(t => t.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .HasConstraintName("FK_RolePermission_Permission_PermissionId");
        }
    }
}
