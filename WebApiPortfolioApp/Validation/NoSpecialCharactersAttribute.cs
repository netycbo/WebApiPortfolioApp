using System.ComponentModel.DataAnnotations;

namespace WebApiPortfolioApp.Validation
{
    public class NoSpecialCharactersAttribute : ValidationAttribute
    {
        private readonly char[] _disallowedCharacters = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '{', '}', '[', ']', '|', '\\', '/', ':', ';', '<', '>', '.', '?' };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var valueAsString = value.ToString();
            if (_disallowedCharacters.Any(c => valueAsString.Contains(c)))
            {
                return new ValidationResult($"The field {validationContext.DisplayName} contains invalid characters.");
            }

            return ValidationResult.Success;
        }
    }
}
