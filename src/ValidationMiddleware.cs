using HotChocolate;
using HotChocolate.Resolvers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Graph.ArgumentValidator
{
    class ValidationMiddleware
    {
        private readonly FieldDelegate _next;

        public ValidationMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var arguments = context.Field.Arguments;

            if (arguments.Count is 0)
                await _next(context);

            var errors = new List<ValidationResult>();

            foreach (var argument in arguments)
            {
                if (argument == null)
                {
                    continue;
                }

                var input = context.ArgumentValue<object>(argument.Name);

                if (input == null)
                {
                    continue;
                }

                if (!ValidatorSettings.ValidateAllInputs &&
                    input.GetType().GetCustomAttribute(typeof(ValidatableAttribute)) == null)
                    continue;

                var validationContext = new ValidationContext(input);
                Validator.TryValidateObject(input, validationContext, errors, true);
            }

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    context.ReportError(ErrorBuilder.New()
                        .SetMessage(error.ErrorMessage)
                        .Build());
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
