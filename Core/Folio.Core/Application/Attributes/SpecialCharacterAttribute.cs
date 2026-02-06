using System.Buffers;
using System.ComponentModel.DataAnnotations;

namespace Folio.Core.Application.Attributes
{
    public class SpecialCharacterAttribute : ValidationAttribute
    {
        private static readonly SearchValues<char> _SpecialCharacterSearchValues = SearchValues.Create("!@#$%^&()_+-={}[]|\\:;\"'<>,.?/");

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            var valueString = value.ToString();

            if (valueString.ContainsAny(_SpecialCharacterSearchValues) is false)
            {
                return new ValidationResult("Password must contain at least one special character");
            }

            return ValidationResult.Success;
        }
    }
}
