using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class EventCommentConfiguration : IEntityTypeConfiguration<EventCommentEntity>
    {
        public void Configure(EntityTypeBuilder<EventCommentEntity> builder)
        {
            builder.HasKey(ec => ec.Id);

            builder.Property(ec => ec.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(ec => ec.CreatedAt)
                .IsRequired();

            builder.HasOne(ec => ec.User)
                .WithMany()
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ec => ec.Event)
                .WithMany(e => e.Comments)
                .HasForeignKey(ec => ec.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
