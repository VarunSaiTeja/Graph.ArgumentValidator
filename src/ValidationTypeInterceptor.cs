using System.Collections.Generic;
using HotChocolate.Configuration;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors.Definitions;

namespace Graph.ArgumentValidator
{
    internal class ValidationTypeInterceptor : TypeInterceptor
    {
        private FieldMiddleware _middleware;

        public override void OnBeforeCompleteType(
            ITypeCompletionContext completionContext,
            DefinitionBase definition,
            IDictionary<string, object> contextData)
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
                            argumentDef.Parameter.IsDefined(typeof(ValidatableAttribute), true))
                        {
                            // we will set a marker for this argument to be validated.
                            argumentDef.ContextData[WellKnownContextData.NeedValidation] = true;
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
                        fieldDef.MiddlewareComponents.Insert(0, _middleware);
                    }
                }
            }
        }
    }

    internal static class WellKnownContextData
    {
        public const string NeedValidation = nameof(NeedValidation);
    }
}
