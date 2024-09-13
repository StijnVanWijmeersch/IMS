using IMS.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IMS.SharedKernel;

namespace IMS.Infrastructure.Categories;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                name => new Name(name));

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(c => c.ProductCategories)
            .WithOne(pc => pc.Category)
            .HasForeignKey(pc => pc.CategoryId);

    }
}
