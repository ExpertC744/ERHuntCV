using System.ComponentModel.DataAnnotations;

namespace HuntCV_Portal.Models
{
    public class CandidateLoginM
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    ErrorMessage = "Enter a valid email address containing @ and ."
)]
        public string? sEmail { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
    ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
    ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string? sPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
