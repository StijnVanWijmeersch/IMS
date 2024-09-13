using IMS.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IMS.SharedKernel;

namespace IMS.Infrastructure.Orders;

internal sealed class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Statuses");

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                name => new Name(name));

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        // Relationships

        builder
            .HasMany(s => s.Orders)
            .WithOne(o => o.Status)
            .HasForeignKey(o => o.StatusId);

    }
}
