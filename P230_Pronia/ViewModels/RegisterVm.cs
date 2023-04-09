using System.ComponentModel.DataAnnotations;

namespace P230_Pronia.ViewModels
{
    public class RegisterVm
    {
        public string Firstname { get; set; }
        [StringLength(maximumLength:15)]
        public string Username { get; set; }

        public string Lastname { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool Terms { get; set; }
    }
}
