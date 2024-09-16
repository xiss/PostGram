using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGram.DAL.Entities;

namespace PostGram.DAL.Configs;

internal class LikeConfig : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasIndex(l => new { l.AuthorId, l.EntityId })
            .IsUnique();

        builder.HasOne(l => l.Author)
            .WithOne()
            .IsRequired(false);
    }
}