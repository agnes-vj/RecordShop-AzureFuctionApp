using System;
using System.ComponentModel.DataAnnotations;

namespace InputBindingDb.Model
{
    public class ReleaseYearValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; 
            }
            if (value is int releaseYear)
            {
                int currentYear = DateTime.Now.Year;
                if (releaseYear > currentYear)
                {
                    return new ValidationResult($"Release Year: {releaseYear} is invalid.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
