using MusicPortal.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace MusicPortal.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Required field First name")]
        [RegularExpression("^[a-zA-Zа-яА-Я0-9_-]+$", ErrorMessage = "The field contains invalid characters.")]
        [StringLength(maximumLength: 20, MinimumLength = 2,
            ErrorMessage = "First name length must be between 2 and 20 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required field Last name")]
        [RegularExpression("^[a-zA-Zа-яА-Я0-9_-]+$", ErrorMessage = "The field contains invalid characters.")]
        [StringLength(maximumLength: 20, MinimumLength = 2,
            ErrorMessage = "Last name length must be between 2 and 20 characters")]
        public string LastName { get; set; }

        [IsUniqueValue(ErrorMessage = "A user with this login already exists.")]
        [Required(ErrorMessage = "Required field Login")]
        [StringLength(maximumLength:20, MinimumLength = 2, 
            ErrorMessage = "Login length must be between 2 and 20 characters")]
        [RegularExpression("[\\w| !\"§$% \\&/ () =\\-?\\@\\*\\.]*", ErrorMessage = "The field contains invalid characters.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Required field Email")]
        [RegularExpression("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}", ErrorMessage = "Invalid mail input.")]
        [IsUniqueValue(ErrorMessage = "A user with this email already exists.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required field Password")]
        [StringLength(maximumLength: 20, MinimumLength = 6,
            ErrorMessage = "Password length must be between 6 and 20 characters")]
        [DataType(DataType.Password)]
        [RegularExpression("[\\w| !\"§$% \\&/ () =\\-?\\@\\*]*", ErrorMessage = "The field contains invalid characters.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required field Confirm password")]
        [Compare("Password", ErrorMessage = "Password mismatch")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}