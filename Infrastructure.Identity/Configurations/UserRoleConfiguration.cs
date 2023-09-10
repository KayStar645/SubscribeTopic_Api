using Core.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.RoleId).IsRequired();

            //builder.HasData(
            //    new UserRoles
            //    {
            //        RoleId = "cbc43a8e-f7bb-4445-baaf-1add431ffbbf",
            //        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
            //    },
            //    new UserRoles
            //    {
            //        RoleId = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
            //        UserId = "9e224968-33e4-4652-b7b7-8574d048cdb9"
            //    }
            //);
        }
    }
}
