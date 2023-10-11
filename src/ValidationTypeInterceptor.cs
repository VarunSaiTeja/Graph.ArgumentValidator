using HotChocolate.Configuration;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors.Definitions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Graph.ArgumentValidator
{
    internal delegate void Validate(object value, ValidationContext validationContext, ICollection<ValidationResult> validationResults);

    internal class ValidationTypeInterceptor : TypeInterceptor
    {
        private FieldMiddleware _middleware;

        public override void OnBeforeCompleteType(
            ITypeCompletionContext completionContext,
            DefinitionBase definition)
        {
            if (definition is ObjectTypeDefinition objectTypeDef)
            {
                foreach (var fieldDef in objectTypeDef.Fields)
                {
                    // most fields do not need validation.
                    bool needValidation = false;

                    foreach (var argumentDef in fieldDef.Arguments)
                    {
                        if (argumentDef.Parameter is not null &&
                            argumentDef.Parameter.IsDefined(typeof(ValidationAttribute), true))
                        {
                            var attributes = argumentDef.Parameter
                                .GetCustomAttributes(typeof(ValidationAttribute), true)
                                .OfType<ValidationAttribute>()
                                .ToArray();

                            // we will set a marker for this argument to be validated.
                            argumentDef.ContextData[WellKnownContextData.ValidationDelegate] =
                                new Validate((value, context, errors) => Validator.TryValidateValue(value, context, errors, attributes));
                            needValidation = true;
                        }
                        else if (argumentDef.Parameter is not null &&
                            argumentDef.Parameter.ParameterType.IsDefined(typeof(ValidatableAttribute), true))
                        {
                            // we will set a marker for this argument to be validated.
                            argumentDef.ContextData[WellKnownContextData.ValidationDelegate] =
                                new Validate((value, context, errors) => Validator.TryValidateObject(value, context, errors, true));
                            needValidation = true;
                        }
                    }

                    if (needValidation)
                    {
                        // if validation is needed we will ensure that a validation middleware exists.
                        if (_middleware is null)
                        {
                            // if no middleware is yet created we will compile a middleware from our
                            // ValidationMiddleware class.
                            _middleware = FieldClassMiddlewareFactory.Create<ValidationMiddleware>();
                        }

                        // we add the validation middleware to the first spot so that validation is executed first.
                        fieldDef.MiddlewareDefinitions.Insert(0, new FieldMiddlewareDefinition(_middleware));
                    }
                }
            }
        }
    }
}
