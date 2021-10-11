# Adds support for validating input arguments in HotChocolate

<a href="https://www.nuget.org/packages/Graph.ArgumentValidator"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/Graph.ArgumentValidator"></a>
<a href="https://www.nuget.org/packages/Graph.ArgumentValidator"><img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/Graph.ArgumentValidator"></a>

Generally, we use attributes from System.ComponentModel.DataAnnotations for validating our input models in controllers.

As HotChocolate doesn't validate input arguments, After installing this package, By just adding 2 lines of code in your Startup.cs file. You will be adding support for validation to all input models in your Queries/Mutations.

[!["Buy Me A Coffee"](https://cdn.buymeacoffee.com/assets/img/home-page-v3/bmc-new-logo.png)](https://www.buymeacoffee.com/varunteja)


**Installation Note**: 

Use Graph.ArgumentValidator v2.0.0 if you are using HotChocolate v12.

Use Graph.ArgumentValidator v1.0.1 if you are using HotChocolate v11.


**Tech Note**: You can use all validation attributes/rules from System.ComponentModel.DataAnnotations (ex: Required, MinLength, Regex etc). This package just adds a middleware to the hot chocolate resolver for validating your input models as configured in the below steps.


## Steps for configuring validator


### Step 1
Go to StartUp.cs file and inside ConfigureServices make the following changes.

### Step 2
Add argument validator to services by referring to below code
```
services
  .AddGraphQLServer()
  .AddArgumentValidator();
```


### Step 3
Just add the `Validatable` attribute to all the classes you defined for input.

Ex:
```
    using System.ComponentModel.DataAnnotations;
    using Graph.ArgumentValidator;
    
    namespace Demo
    {
        [Validatable]
        public class RegisterUserInput
        {
            [Required, MinLength(4, ErrorMessage = "Username must be atleast 4 characters.")]
            public string UserName { get; set; }

            [Required, EmailAddress(ErrorMessage = "Email Id format is invalid.")]
            public string Email { get; set; }

            [Required, RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password needs to be more strong.")]
            public string Password { get; set; }
            
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
            public string ConfirmPassword { get; set; }
        }
        
        public class UserMutations
        {
            public int RegisterUser(RegisterUserInput input, [Service] UserService userService)
            {
                return userService.RegisterUser(input);
            }
        }
    }
```

Another way of providing inline validation for Primitive data types
```
  using System.ComponentModel.DataAnnotations;
  using Graph.ArgumentValidator;
  
  namespace Demo
  {
      public class Mutations
      {
          public bool RegisterUserPhone([Required(ErrorMessage = "Email is required")string email,
              [Phone(ErrorMessage = "Invalid Phone Number")] string phone)
          {
              return userService.RegisterUserPhone(email,phone);
          }
      }
  }
```


When the user is given the following wrong values to the mutation input
 ```
      mutation{
       registerUser(input:{userName:"va2", password:"weak", confirmPassword:"strong", email:"varun"})
      }
 ```
 
This is the response we got from GraphQL Server
```
{
  "errors": [
    {
      "message": "Username must be atleast 4 characters.",
      "path": [
        "input"
      ],
      "extensions": {
        "field": "userName"
      }
    },
    {
      "message": "Email Id format is invalid.",
      "path": [
        "input"
      ],
      "extensions": {
        "field": "email"
      }
    },
    {
      "message": "Password needs to be more strong.",
      "path": [
        "input"
      ],
      "extensions": {
        "field": "password"
      }
    },
    {
      "message": "Passwords do not match!",
      "path": [
        "input"
      ],
      "extensions": {
        "field": "confirmPassword"
      }
    }
  ]
}
```
