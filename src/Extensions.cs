using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Graph.ArgumentValidator
{
    public static class Extensions
    {
        public static IRequestExecutorBuilder AddArgumentValidator(
            this IRequestExecutorBuilder requestExecutorBuilder)
        {
            requestExecutorBuilder.UseField<ValidationMiddleware>();

            return requestExecutorBuilder;
        }
    }
}
