using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SubscribeTopicDbContext>
    {
        public SubscribeTopicDbContext CreateDbContext(string[] args) 
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SubscribeTopicDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SubscribeTopicConnectionString"));

            return new SubscribeTopicDbContext(optionsBuilder.Options);
        }
    }
}
