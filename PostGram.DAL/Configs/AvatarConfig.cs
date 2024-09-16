using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGram.DAL.Entities;

namespace PostGram.DAL.Configs;

internal class AvatarConfig : IEntityTypeConfiguration<Avatar>
{
    public void Configure(EntityTypeBuilder<Avatar> builder)
    {
        builder.ToTable(nameof(DataContext.Avatars));
        builder.HasIndex(a => a.UserId).IsUnique();
        builder.HasOne(a => a.User)
            .WithOne(u => u.Avatar)
            .HasForeignKey(nameof(User));
    }
}