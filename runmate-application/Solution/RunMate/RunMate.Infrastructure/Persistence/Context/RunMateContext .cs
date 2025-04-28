using Microsoft.EntityFrameworkCore;
using RunMate.Authentication.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration;
using RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration;

namespace RunMate.Authentication.RunMate.Infrastructure.Persistence.Context
{
    public class RunMateContext(DbContextOptions<RunMateContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<EventParticipantEntity> EventParticipants { get; set; }
        public DbSet<EventCheckpointEntity> EventCheckpoints { get; set; }
        public DbSet<EventCommentEntity> EventComments { get; set; }
        public DbSet<PublicationEntity> Publications { get; set; }
        public DbSet<PublicationCommentEntity> PublicationComments { get; set; }
        public DbSet<PublicationLikeEntity> PublicationLikes { get; set; }
        public DbSet<UserFriendEntity> UserFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações existentes
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Novas configurações para eventos
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new EventParticipantConfiguration());
            modelBuilder.ApplyConfiguration(new EventCheckpointConfiguration());
            modelBuilder.ApplyConfiguration(new EventCommentConfiguration());

            // Configurações para o Feed de Publicações
            modelBuilder.ApplyConfiguration(new PublicationConfiguration());
            modelBuilder.ApplyConfiguration(new PublicationCommentConfiguration());
            modelBuilder.ApplyConfiguration(new PublicationLikeConfiguration());

            modelBuilder.ApplyConfiguration(new UserFriendConfiguration());
        }
    }
}