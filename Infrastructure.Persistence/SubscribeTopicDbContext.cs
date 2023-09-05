﻿using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class SubscribeTopicDbContext : AuditableDbContext
    {
        public SubscribeTopicDbContext(DbContextOptions<SubscribeTopicDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubscribeTopicDbContext).Assembly);
        }

        public DbSet<Teacher> Teachers { get; set; }
    }
}
