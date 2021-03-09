using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Graph.ArgumentValidator
{
    public static class RequestExecutorBuilderExtensions
    {
        public static IRequestExecutorBuilder AddArgumentValidator(
            this IRequestExecutorBuilder requestExecutorBuilder)
        {
            requestExecutorBuilder.TryAddTypeInterceptor<ValidationTypeInterceptor>();

            return requestExecutorBuilder;
        }
    }
}
