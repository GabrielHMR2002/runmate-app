using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.UserService.RunMate.Infrastructure.Persistence.Configurations.DomainConfiguration
{
    public class UserFriendConfiguration : IEntityTypeConfiguration<UserFriendEntity>
    {
        public void Configure(EntityTypeBuilder<UserFriendEntity> builder)
        {
            builder.ToTable("UserFriends");

            builder.HasKey(uf => uf.Id);

            builder.Property(uf => uf.Id)
                .ValueGeneratedNever();

            builder.Property(uf => uf.CreatedAt)
                .IsRequired();

            // Relações
            builder.HasOne(uf => uf.User)
                .WithMany()
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.Friend)
                .WithMany()
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.Friend)
                .WithMany(u => u.FriendOf)
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(uf => new { uf.UserId, uf.FriendId }).IsUnique();
        }
    }
}
