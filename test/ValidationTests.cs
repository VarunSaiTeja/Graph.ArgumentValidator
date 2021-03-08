using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
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
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync("{ argIsInput(input: { email: \"abc@abc.com\" }) }");

            result.ToJson().MatchSnapshot();
        }
    }

    public class Query
    {
        public string ArgIsEmail([EmailAddress] string email) => email;

        public string ArgIsInput(MyInput input) => input.Email;
    }

    [Validatable]
    public class MyInput
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
