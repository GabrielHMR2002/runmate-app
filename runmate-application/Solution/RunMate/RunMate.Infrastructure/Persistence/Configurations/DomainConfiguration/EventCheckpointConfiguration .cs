using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class EventCheckpointConfiguration : IEntityTypeConfiguration<EventCheckpointEntity>
    {
        public void Configure(EntityTypeBuilder<EventCheckpointEntity> builder)
        {
            builder.HasKey(ec => ec.Id);

            builder.Property(ec => ec.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ec => ec.Latitude)
                .IsRequired();

            builder.Property(ec => ec.Longitude)
                .IsRequired();

            builder.Property(ec => ec.DistanceFromStart)
                .IsRequired();

            builder.Property(ec => ec.Order)
                .IsRequired();

            builder.HasOne(ec => ec.Event)
                .WithMany(e => e.Checkpoints)
                .HasForeignKey(ec => ec.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
