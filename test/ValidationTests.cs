using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Graph.ArgumentValidator
{
    public class ValidationTests
    {
        [Fact]
        public async Task Ensure_Validation_Works_On_Arguments()
        {
            var result =
                await new ServiceCollection()
                    .AddScoped(_ => new Service())
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync("{ argIsEmail(email: \"abc\") }");

            result.ToJson().MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_Arguments_ValidEmail()
        {
            var result =
                await new ServiceCollection()
                    .AddScoped(_ => new Service())
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync("{ argIsEmail(email: \"abc@abc.com\") }");

            result.ToJson().MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_InputObjects()
        {
            var result =
                await new ServiceCollection()
                    .AddScoped(_ => new Service())
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync("{ argIsInput(input: { email: \"abc\" }) }");

            result.ToJson().MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_InputObjects_ValidEmail()
        {
            var result =
                await new ServiceCollection()
                    .AddScoped(_ => new Service())
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync("{ argIsInput(input: { email: \"abc@abc.com\" }) }");

            result.ToJson().MatchSnapshot();
        }
    }

    public class Query
    {
        public string ArgIsEmail([EmailAddress][ResolveService] string email) => email;

        public string ArgIsInput(MyInput input) => input.Email;
    }

    [Validatable]
    public class MyInput
    {
        [EmailAddress]
        [ResolveService]
        public string Email { get; set; }
    }

    public class Service
    {
        public bool CouldBeResolved => true;
    }

    public class ResolveServiceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = (Service)validationContext.GetService(typeof(Service));
            return service is { CouldBeResolved: true } ? ValidationResult.Success : new ValidationResult("error");
        }
    }
}
