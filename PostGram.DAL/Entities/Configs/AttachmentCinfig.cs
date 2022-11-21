using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
    internal class AttachmentCinfig : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasOne(a => a.Author)
                .WithMany()
                .IsRequired(false);
        }
    }
}