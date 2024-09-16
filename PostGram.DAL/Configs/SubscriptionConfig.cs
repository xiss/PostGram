using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGram.DAL.Entities;

namespace PostGram.DAL.Configs;

internal class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder
            .HasIndex(s => new { s.MasterId, s.SlaveId })
            .IsUnique();
    }
}