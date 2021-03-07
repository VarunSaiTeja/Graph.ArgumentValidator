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
        public async Task Ensure_Validation_Is_Executed()
        {
            var result =
                await new ServiceCollection()
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync("{ argIsEmail(email: \"abc\") }");
            
            result.ToJson().MatchSnapshot();
        }
    }

    public class Query {
        public string ArgIsEmail([Validatable] [EmailAddress] string email) => email;
    }
}
