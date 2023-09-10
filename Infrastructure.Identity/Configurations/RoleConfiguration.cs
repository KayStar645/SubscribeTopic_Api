using Core.Application.Config;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            //builder.HasData(
            //    new Roles
            //    {
            //        Id = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
            //        Name = RoleConfig.Ministry(),
            //        NormalizedName = RoleConfig.Ministry().ToUpper()
            //    },
            //    new Roles
            //    {
            //        Id = "cbc43a8e-f7bb-4445-baaf-1add431ffbbf",
            //        Name = RoleConfig.Admin(),
            //        NormalizedName = RoleConfig.Admin().ToUpper()
            //    }
            //);
        }
    }
}
