using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
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
}