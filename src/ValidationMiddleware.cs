using HotChocolate;
using HotChocolate.Resolvers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Graph.ArgumentValidator
{
    internal class ValidationMiddleware
    {
        internal class ErrorDetail
        {
            public string ErrorMessage;
            public string Field;
            public string Path;
        }

        private readonly FieldDelegate _next;

        public ValidationMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        // this middleware is ensured to only execute on fields that have arguments that need validation.
        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var validationResults = new List<ValidationResult>();
            var errors = new List<ErrorDetail>();

            // we could even further optimize and aggregate this list in the interceptor and inject it into the middleware
            foreach (var argument in context.Field.Arguments)
            {
                if (argument.ContextData.TryGetValue(WellKnownContextData.ValidationDelegate, out object value) &&
                    value is Validate validate)
                {
                    var input = context.ArgumentValue<object>(argument.Name);
                    var validationContext = new ValidationContext(input);
                    validate(input, validationContext, validationResults);

                    if (validationResults.Any())
                    {
                        foreach (var validationResult in validationResults)
                        {
                            errors.Add(new ErrorDetail
                            {
                                ErrorMessage = validationResult.ErrorMessage,
                                Field = validationResult.MemberNames.FirstOrDefault() ?? argument.Name,
                                Path = argument.Name
                            });
                        }
                        validationResults.Clear();
                    }
                }
            }

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    // NOTE: it would be good to provide an error code.
                    context.ReportError(ErrorBuilder.New()
                        .SetMessage(error.ErrorMessage)
                        .SetExtension("field", char.ToLowerInvariant(error.Field[0]) + error.Field.Substring(1))
                        .SetPath(Path.New(error.Path))
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
