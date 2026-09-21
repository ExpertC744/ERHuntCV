using System.ComponentModel.DataAnnotations;

namespace ERHuntCV.Models
{
    public class SuperAdminResetPassword
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string sEmail { get; set; } = string.Empty;


        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters."
        )]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage =
                "Password must contain uppercase, lowercase, number and special character."
        )]
        public string NewPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare(
            "NewPassword",
            ErrorMessage = "New password and confirm password do not match."
        )]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}