using System.ComponentModel.DataAnnotations;

namespace Sefia.Attributes;

public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string password)
        {
            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasNumber = password.Any(char.IsDigit);

            if (!hasUpperCase)
                return new ValidationResult("Password must contain at least one uppercase letter.");
            if (!hasLowerCase)
                return new ValidationResult("Password must contain at least one lowercase letter.");
            if (!hasNumber)
                return new ValidationResult("Password must contain at least one number.");
        }
        return ValidationResult.Success!;
    }
}
