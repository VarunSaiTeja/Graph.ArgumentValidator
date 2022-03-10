using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class DuplicateEmailValidtorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object valueObj, ValidationContext validationContext)
        {
            var value = valueObj as string;
            var service = (DuplicateEmailValidatorService)validationContext.GetService(typeof(DuplicateEmailValidatorService));
            return service.IsEmailExist(value) ? ValidationResult.Success : new ValidationResult("Email already exist");
        }
    }
}
