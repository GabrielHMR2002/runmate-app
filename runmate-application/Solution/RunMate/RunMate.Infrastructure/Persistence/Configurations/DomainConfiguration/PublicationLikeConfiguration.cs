using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class PublicationLikeConfiguration : IEntityTypeConfiguration<PublicationLikeEntity>
    {
        public void Configure(EntityTypeBuilder<PublicationLikeEntity> builder)
        {
            builder.ToTable("PublicationLikes");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.CreatedAt)
                .IsRequired();

            builder.HasOne(l => l.Publication)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(l => new { l.PublicationId, l.UserId })
                .IsUnique();
        }
    }
}
