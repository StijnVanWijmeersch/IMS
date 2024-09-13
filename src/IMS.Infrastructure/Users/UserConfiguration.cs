using IMS.Domain.Customers;
using IMS.Domain.Users;
using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("ApplicationUsers");

        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasConversion(
                firstName => firstName.Value,
                firstName => new Name(firstName));

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasConversion(
                lastName => lastName.Value,
                lastName => new Name(lastName));

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasConversion(
                email => email.Value,
                email => new Email(email));

        builder.Property(u => u.HashedPassword)
            .IsRequired()
            .HasConversion(
                password => password.Value,
                password => new Password(password));

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);
    }
}
