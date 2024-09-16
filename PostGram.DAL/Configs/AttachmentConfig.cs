using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGram.DAL.Entities;

namespace PostGram.DAL.Configs;
// TODO Вопрос, если переносить Entity в d BLL то что делать с контекстом?
internal class AttachmentConfig : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasOne(a => a.Author)
            .WithMany()
            .IsRequired(false);
    }
}