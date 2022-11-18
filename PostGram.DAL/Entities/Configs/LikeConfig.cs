using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
    internal class LikeConfig : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            // Один индекс по трем полям не работает, так как поля могут содержать null
            builder.HasIndex(u => new { u.AuthorId, u.PostId }).IsUnique();
            builder.HasIndex(u => new { u.AuthorId, u.CommentId }).IsUnique();
        }
    }
}