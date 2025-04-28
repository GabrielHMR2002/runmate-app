using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;
using RunMate.UserService.RunMate.Domain.Enums;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class PublicationConfiguration : IEntityTypeConfiguration<PublicationEntity>
    {
        public void Configure(EntityTypeBuilder<PublicationEntity> builder)
        {
            builder.ToTable("Publications");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.Property(p => p.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.Visibility)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PublicationVisibility.Public);

            builder.HasOne(p => p.Author)
                .WithMany(u => u.Publications)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.AuthorId);
            builder.HasIndex(p => p.CreatedAt);
            builder.HasIndex(p => p.Type);
            builder.HasIndex(p => p.Visibility);
        }
    }
}
