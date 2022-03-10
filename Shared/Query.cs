using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class Query
    {
        public string ArgIsEmail([EmailAddress, Required] string email) => email;

        /// <summary>
        /// Gives validation failed result if email already exist. Other wise *You are good to go...*
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string CheckDuplicateEmail([EmailAddress, Required][DuplicateEmailValidtor] string email) => "You are good to go, this email not registred yet.";

        public string ArgIsInput(MyInput input) => input.Email;
    }
}
