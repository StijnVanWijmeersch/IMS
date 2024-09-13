using System.Diagnostics.CodeAnalysis;

namespace IMS.SharedKernel;

public static class EnsureThat
{
    public static void IsPositive(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Value must be a positive number", nameof(value));
        }
    }

    public static void IsNotNullOrEmpty([NotNull] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(value, "Value cannot be null or empty");
        }
    }

    public static void IsValidDescription([NotNull] string value)
    {
        if (value.Length > 1000)
        {
            throw new ArgumentException("Description cannot be longer than 250 characters", value);
        }
    }

    public static void IsValidImage([NotNull] string value)
    {
        if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
        {
            throw new ArgumentException("Image URI must be a valid URI", value);
        }
    }

    public static void StockValueIsValid(int stockQuantity, bool inStock)
    {
        if (stockQuantity == 0 && inStock || stockQuantity > 0 && !inStock)
        {
            throw new ArgumentException("Stock quantity and availablility does not match", nameof(stockQuantity));
        }
    }

    public static void IsValidEmail(string value)
    {
        var trimmedEmail = value.Trim();

        if (trimmedEmail.EndsWith('.'))
        {
            throw new ArgumentException("Email is not valid", nameof(value));
        }

        try
        {
            if (!new System.Net.Mail.MailAddress(value).Address.Equals(value))
            {
                throw new ArgumentException("Email is not valid", nameof(value));
            }
        }
        catch
        {
            throw new ArgumentException("Email is not valid", nameof(value));
        }
    }

    public static void IsValidPassword(string value)
    {
        if (value.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long", nameof(value));
        }

        if (!value.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one digit", nameof(value));
        }

        if (!value.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter", nameof(value));
        }

        if (!value.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter", nameof(value));
        }

        if (value.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("Password cannot contain whitespace", nameof(value));
        }
    }
}
