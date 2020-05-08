using System.ComponentModel.DataAnnotations;

namespace MusicPortal.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required field")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Password)]
        [RegularExpression("[\\w| !\"§$% \\&/ () =\\-?\\@\\*]*", ErrorMessage = "The field contains invalid characters.")]
        public string Password { get; set; }
    }
}