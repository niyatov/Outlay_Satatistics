using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace outlay_mvc.Entities.Configurations;

public class CategoryConfiguration : EntityBaseConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        builder.Property(b => b.Key).HasMaxLength(40);
        builder.Property(b => b.Name).IsUnicode().IsRequired().HasMaxLength(20);
    }
}