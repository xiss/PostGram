using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGram.DAL.Entities;

namespace PostGram.DAL.Configs;

internal class PostContentConfig : IEntityTypeConfiguration<PostContent>
{
    public void Configure(EntityTypeBuilder<PostContent> builder)
    {
        builder.ToTable(nameof(DataContext.PostContents));
        builder.HasOne(x => x.Post)
            .WithMany(x => x.PostContents)
            .HasForeignKey(x => x.PostId)
            .IsRequired(false);
    }
}