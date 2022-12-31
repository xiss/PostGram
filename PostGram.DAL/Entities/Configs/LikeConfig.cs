using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
    internal class LikeConfig : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasIndex(u => new { u.AuthorId, u.EntityId })
                .IsUnique();

            builder.HasOne(l => l.Author)
                .WithOne()
                .IsRequired(false);
        }
    }
}