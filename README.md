# Adds support for validating input arguments in HotChocolate

<a href="https://www.nuget.org/packages/Graph.ArgumentValidator"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/Graph.ArgumentValidator"></a>
<a href="https://www.nuget.org/packages/Graph.ArgumentValidator"><img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/Graph.ArgumentValidator"></a>

Generally, we use attributes from System.ComponentModel.DataAnnotations for validating our input models in controllers.

As HotChocolate doesn't validate input arguments, After installing this package, By just adding 2 lines of code in your Startup.cs file. You will be adding support for validation to all input models in your Queries/Mutations.

**Tech Note**: You can use all validation attributes/rules from System.ComponentModel.DataAnnotations (ex: Required, MinLength, Regex etc). This package just adds a middleware to the hot chocolate resolver for validating your input models as configured below steps.


## Steps for configuring validator


### Step 1
Go to StartUp.cs file and inside ConfigureServices make the following changes.

### Step 2
Keep this code to validate all inputs of GraphQL Query/Mutation.
```
ValidatorSettings.ValdiateAllInputs = true;
```


### Step 3
Add argument validator to services by referring to below code
```
services
  .AddGraphQLServer()
  .AddArgumentValidator();
```


### Additional Configuration
If you don't want to validate all inputs then you can skip step 2 and follow the optional step i.e

Just add the `Validatable` attribute to the class u defined for input.

Ex:
```
    using System.ComponentModel.DataAnnotations;
    using Graph.ArgumentValidator;
    
    namespace Demo
    {
        [Validatable]
        public class AddUserInput
        {
            [Required, MinLength(4, ErrorMessage = "Username must be atleast 4 characters.")]
            public string UserName { get; set; }

            [Required, RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email Id format is invalid")]
            public string Email { get; set; }

            [Required, RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password needs to be more strong.")]
            public string Password { get; set; }
        }
    }
```


When the user is given wrong values to input, this is how the response from GraphQL Server will look like
```
{
  "errors": [
    {
      "message": "Username must be at least 4 characters."
    },
    {
      "message": "Password needs to be more strong."
    }
  ]
}
```
