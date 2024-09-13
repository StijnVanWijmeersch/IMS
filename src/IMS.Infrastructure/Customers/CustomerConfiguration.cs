using IMS.Domain.Customers;
using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasConversion(
                firstName => firstName.Value,
                firstName => new Name(firstName));

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasConversion(
                lastName => lastName.Value,
                lastName => new Name(lastName));

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.Email)
            .IsRequired()
            .HasConversion(
                email => email.Value,
                email => new Email(email));

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        // Relationships

        builder
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);
    }
}
