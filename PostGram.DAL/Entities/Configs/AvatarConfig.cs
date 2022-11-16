using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
    internal class AvatarConfig : IEntityTypeConfiguration<Avatar>
    {
        public void Configure(EntityTypeBuilder<Avatar> builder)
        {
            builder.ToTable(nameof(DataContext.Avatars));
            builder.HasIndex(a => a.UserId).IsUnique(); //TODO 1 При вставке аватара не создается индекс у аватара, проверить
            builder.HasOne(a => a.User)
                .WithOne(u => u.Avatar)
                .HasForeignKey(nameof(User));

        }
    }
}