using Microsoft.Extensions.DependencyInjection;

using HotChocolate.Execution.Configuration;

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
