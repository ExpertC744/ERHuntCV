using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class SALoginM
    {
        public int nID { get; set; }

        public int SAID { get; set; }

        public string? sFName { get; set; }

        public string? sLName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(
     @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
     ErrorMessage = "Enter a valid email address containing @ and ."
 )]
        public string sEmail { get; set; }

        public string? sMobile { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
      ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(
      @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
      ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string sPassword { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime? ModDate { get; set; }

        public bool nBit { get; set; }

        public string? sRole { get; set; }
    }
}
