using System.ComponentModel.DataAnnotations;

namespace Course_Project.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
