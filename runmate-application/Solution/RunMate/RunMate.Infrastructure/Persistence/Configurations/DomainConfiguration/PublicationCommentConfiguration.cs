using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class PublicationCommentConfiguration : IEntityTypeConfiguration<PublicationCommentEntity>
    {
        public void Configure(EntityTypeBuilder<PublicationCommentEntity> builder)
        {
            builder.ToTable("PublicationComments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(c => c.Publication)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(c => c.PublicationId);
            builder.HasIndex(c => c.AuthorId);
            builder.HasIndex(c => c.CreatedAt);
        }
    }
}
