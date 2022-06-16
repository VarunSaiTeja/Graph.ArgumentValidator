using HotChocolate;
using HotChocolate.Resolvers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Graph.ArgumentValidator
{
    internal class ValidationMiddleware
    {
        private readonly FieldDelegate _next;

        public ValidationMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        // this middleware is ensured to only execute on fields that have arguments that need validation.
        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var errors = new List<ValidationResult>();
            var hasErrors = false;

            // we could even further optimize and aggregate this list in the interceptor and inject it into the middleware
            foreach (var argument in context.Selection.Field.Arguments)
            {
                if (argument.ContextData.TryGetValue(WellKnownContextData.ValidationDelegate, out object value) &&
                    value is Validate validate)
                {
                    var input = context.ArgumentValue<object>(argument.Name);
                    var validationContext = new ValidationContext(input, context.Services, null);
                    RecursiveValidate(input, context, errors);

                    if (errors.Any())
                    {
                        foreach (var validationResult in errors)
                        {
                            var field = validationResult.MemberNames.FirstOrDefault() ?? argument.Name;

                            context.ReportError(ErrorBuilder.New()
                                .SetMessage(validationResult.ErrorMessage)
                                .SetExtension("field", char.ToLowerInvariant(field[0]) + field.Substring(1))
                                .SetPath(Path.New(argument.Name))
                                .Build());
                        }

                        errors.Clear();
                        hasErrors = true;
                    }
                }
            }

            if (!hasErrors)
            {
                await _next(context);
            }
        }

        public void RecursiveValidate(object input, IMiddlewareContext context, List<ValidationResult> errors)
        {
            var validationContext = new ValidationContext(input, context.Services, null);
            Validator.TryValidateObject(input, validationContext, errors, true);

            PropertyInfo[] props = input.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object propInput = input.GetType().GetProperty(prop.Name).GetValue(input, null);
                if (propInput != null && propInput.GetType().Namespace != "System")
                    RecursiveValidate(propInput, context, errors);
            }

        }
    }
}
