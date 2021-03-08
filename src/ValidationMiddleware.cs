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

            // we could even further optimize and aggregate this list in the interceptor and inject it into the middleware
            foreach (var argument in context.Field.Arguments)
            {
                if (argument.ContextData.TryGetValue(WellKnownContextData.ValidationDelegate, out object value) &&
                    value is Validate validate)
                {
                    var input = context.ArgumentValue<object>(argument.Name);
                    var validationContext = new ValidationContext(input);
                    validate(input, validationContext, errors);
                }
            }

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    // NOTE: it would be good to provide an error code.
                    context.ReportError(ErrorBuilder.New()
                        .SetMessage(error.ErrorMessage)
                        // .SetCode()
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
