using Graph.ArgumentValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    [Validatable]
    public class Student
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required]
        public ScoreCardInfo ScoreCard { get; set; }

        public class ScoreCardInfo
        {
            [Required]
            public int TotalScore { get; set; }

            [Required(AllowEmptyStrings = false)]
            public string School { get; set; }
        }
    }
}
