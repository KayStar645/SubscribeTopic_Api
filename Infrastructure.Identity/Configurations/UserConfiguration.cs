using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable(nameof(Users));

            builder.HasKey(x => x.Id);


            //var hasher = new PasswordHasher<Users>();
            //builder.HasData(
            //     new Users
            //     {
            //         Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
            //         Email = "admin@gmail.com",
            //         NormalizedEmail = "ADMIN@GMAIL.COM",
            //         UserName = "admin",
            //         NormalizedUserName = "ADMIN",
            //         PasswordHash = hasher.HashPassword(null, "123456789"),
            //         EmailConfirmed = true
            //     },
            //     new Users
            //     {
            //         Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
            //         Email = "thuanpt182@gmail.com",
            //         NormalizedEmail = "THUANPT182@GMAIL.COM",
            //         UserName = "thuanpt182",
            //         NormalizedUserName = "THUANPT182",
            //         PasswordHash = hasher.HashPassword(null, "123456789"),
            //         EmailConfirmed = true
            //     }
            //);
        }
    }
}
