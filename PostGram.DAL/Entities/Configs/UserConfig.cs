using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasMany(u => u.Slaves)
                .WithOne(s => s.Slave)
                .HasForeignKey(s => s.SlaveId);

            builder.HasMany(u => u.Masters)
                .WithOne(s => s.Master)
                .HasForeignKey(s => s.MasterId);
        }
    }
}