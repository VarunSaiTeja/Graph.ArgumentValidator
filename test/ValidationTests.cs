using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Graph.ArgumentValidator
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
                    .AddScoped(_ => new DataValidatorService())
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

    public class Query
    {
        public string ArgIsEmail([EmailAddress] string email) => email;

        /// <summary>
        /// Gives validation failed result if email already exist. Other wise *You are good to go...*
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string CheckDuplicateEmail([EmailAddress][DuplicateEmailValidtor] string email) => "You are good to go, this email not registred yet.";

        public string ArgIsInput(MyInput input) => input.Email;
    }

    [Validatable]
    public class MyInput
    {
        [EmailAddress]
        public string Email { get; set; }
    }

    public class DataValidatorService
    {
        public bool IsEmailExist(string newEmail)
        {
            var existingEmails = new List<string>
            {
                "varun@gmail.com",
                "teja@gmail.com"
            };

            return !existingEmails.Contains(newEmail);
        }
    }

    public class DuplicateEmailValidtorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object valueObj, ValidationContext validationContext)
        {
            var value = valueObj as string;
            var service = (DataValidatorService)validationContext.GetService(typeof(DataValidatorService));
            return service.IsEmailExist(value) ? ValidationResult.Success : new ValidationResult("Email already exist");
        }
    }
}
