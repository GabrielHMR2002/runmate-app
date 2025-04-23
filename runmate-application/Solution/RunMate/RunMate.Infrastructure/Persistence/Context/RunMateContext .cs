using Microsoft.EntityFrameworkCore;
using RunMate.Authentication.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration;
using RunMate.Domain.Entities;

namespace RunMate.Authentication.RunMate.Infrastructure.Persistence.Context
{
    public class RunMateContext(DbContextOptions<RunMateContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}