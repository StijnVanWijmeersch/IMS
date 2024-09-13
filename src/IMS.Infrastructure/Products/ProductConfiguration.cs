using IMS.Domain.Products;
using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Products;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder
           .HasQueryFilter(p => !p.IsDeleted);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                name => new Name(name));

        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasConversion(
                description => description.Value,
                description => new Description(description));

        builder.Property(p => p.ShortDescription)
            .IsRequired(false)
            .HasConversion(
                shortDescription => shortDescription.Value,
                shortDescription => new Description(shortDescription));

        builder.HasIndex(p => p.Sku)
            .IsUnique();
        builder.Property(p => p.Sku)
            .IsRequired()
            .HasConversion(
                sku => sku.Value,
                sku => new Sku(sku));

        builder.Property(p => p.InStock)
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasPrecision(18, 4)
            .IsRequired()
            .HasConversion(
                price => price.Value,
                price => new Price(price));

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(p => p.Image)
            .IsRequired()
            .HasConversion(
                image => image.Value,
                image => new ImageUrl(image));

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder
            .HasMany(p => p.ProductCategories)
            .WithOne(pc => pc.Product)
            .HasForeignKey(pc => pc.ProductId);

    }

}
