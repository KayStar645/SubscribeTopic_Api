using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Identity
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SubcribeTopicIdentityDbContext>
    {
        public SubcribeTopicIdentityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SubcribeTopicIdentityDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SubscribeTopicIdentityConnectionString"));

            return new SubcribeTopicIdentityDbContext(optionsBuilder.Options);
        }
    }
}
