using Microsoft.EntityFrameworkCore;
using RunMate.EngagementService.Domain.Models;
using RunMate.EngagementService.RunMate.EngagementService.Infrastructure.Persistence.Configuration.DomainConfiguration;

namespace RunMate.EngagementService.RunMate.EngagementService.Infrastructure.Persistence
{
    public class RunMateEngagementDbContext(DbContextOptions<RunMateEngagementDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}


