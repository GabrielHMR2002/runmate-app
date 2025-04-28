using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipantEntity>
    {
        public void Configure(EntityTypeBuilder<EventParticipantEntity> builder)
        {
            builder.HasKey(ep => ep.Id);

            builder.HasIndex(ep => new { ep.EventId, ep.UserId })
                .IsUnique();

            builder.Property(ep => ep.RegistrationDate)
                .IsRequired();

            builder.HasOne(ep => ep.User)
                .WithMany(u => u.EventParticipations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
