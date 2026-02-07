using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.Attributes
{
    public class ContainsUppercaseAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            var valueString = value.ToString();

            bool containsUppercase = valueString!.Any(char.IsUpper);

            if (containsUppercase != true)
            {
                return new ValidationResult("Password must have at least one uppercase letter");
            }

            return ValidationResult.Success;
        }
    }
}
