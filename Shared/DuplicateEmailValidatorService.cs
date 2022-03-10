using System.Collections.Generic;

namespace Shared
{
    public class DuplicateEmailValidatorService
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
}
