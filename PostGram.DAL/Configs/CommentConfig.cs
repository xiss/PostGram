using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGram.DAL.Entities;

namespace PostGram.DAL.Configs;

internal class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasOne(c => c.Post).WithMany(p => p.Comments);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}