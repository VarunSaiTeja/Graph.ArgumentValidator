using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Snapshooter.Xunit;
using System.Threading.Tasks;
using Xunit;

namespace Graph.ArgumentValidator.Tests
{
    public class ValidationTests
    {
        /// <summary>
        /// Gives the JSON result of execution
        /// </summary>
        /// <param name="request">GraphQL query to be executed</param>
        /// <returns>JSON result of GraphQL Query</returns>
        static async Task<string> ExecuteRequest(string request)
        {
            var resonse = await new ServiceCollection()
                    .AddScoped(_ => new DuplicateEmailValidatorService())
                    .AddGraphQL()
                    .AddQueryType<Query>()
                    .AddArgumentValidator()
                    .ExecuteRequestAsync(request);
            return resonse.ToJson();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_Arguments()
        {
            var result = await ExecuteRequest("{ argIsEmail(email: \"abc\") }");

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_Arguments_ValidEmail()
        {
            var result = await ExecuteRequest("{ argIsEmail(email: \"abc@abc.com\") }");

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_InputObjects()
        {
            var result = await ExecuteRequest("{ argIsInput(input: { email: \"abc\" }) }");

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_InputObjects_ValidEmail()
        {
            var result = await ExecuteRequest("{ argIsInput(input: { email: \"abc@abc.com\" }) }");

            result.MatchSnapshot();
        }


        [Fact]
        public async Task Ensure_Validation_Works_On_DuplicateEmail()
        {
            var result = await ExecuteRequest("{ checkDuplicateEmail( email: \"varun@gmail.com\" ) }");

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Ensure_Validation_Works_On_NonDuplicateEmail()
        {
            var result = await ExecuteRequest("{ checkDuplicateEmail( email: \"sai@gmail.com\" ) }");

            result.MatchSnapshot();
        }
    }
}
