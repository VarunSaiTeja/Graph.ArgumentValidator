using Graph.ArgumentValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    [Validatable]
    public class MyInput
    {
        [EmailAddress, Required]
        public string Email { get; set; }
    }
}
