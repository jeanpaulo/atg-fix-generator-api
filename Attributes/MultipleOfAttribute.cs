using System.ComponentModel.DataAnnotations;

namespace OrderGenerator.API.Attributes;
public class MultipleOfAttribute : ValidationAttribute
{
    private readonly decimal _factor;

    public MultipleOfAttribute(double factor)
    {
        _factor = (decimal)factor;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var decimalValue = Convert.ToDecimal(value);
        if (decimalValue % _factor == 0)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"The field {validationContext.DisplayName} must be a multiple of {_factor}.");
    }
}
