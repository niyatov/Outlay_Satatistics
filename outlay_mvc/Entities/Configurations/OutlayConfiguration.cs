using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace outlay_mvc.Entities.Configurations;

public class OutlayConfiguration : EntityBaseConfiguration<Outlay>
{
    public override void Configure(EntityTypeBuilder<Outlay> builder)
    {
        base.Configure(builder);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(20);
        builder.Property(b => b.Cost).IsRequired(true);
    }
}