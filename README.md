# Adds support for validating input arguments in HotChocolate

Generally we use attributes from System.Annotations for validating our input models in controllers.

As HotChocolate don't validate input arguments , After installing this package, By just adding 2 lines of code in your Startup.cs file. You will be adding support for validation to all input models in your Queries/Mutations.


## Steps for configuring validator


### Step 1
Go to StartUp.cs file and inside ConfigureServices make the following changes.

### Step 2
Keep this code to validate all inputs of GraphQL Query/Mutation.
```
ValidatorSettings.ValdiateAllInputs = true;
```


### Step 3
Add argument valdiator to services by referring below code
```
services
  .AddGraphQLServer()
  .AddArgumentValidator();
```


### Additional Configuration
If you don't want to valdiate all inputs then you can skip step 2 and follow the optional step i.e

Just add `Validatable` attribute to the class u defined for input.

Ex:
```
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
```


When user given wrong values to input, this is how the response from GraphQL Server will looks like
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
