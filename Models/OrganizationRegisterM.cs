using System.ComponentModel.DataAnnotations;

namespace HuntCV_Portal.Models
{
    public class OrganizationRegisterM
    {
        public int nID { get; set; }


        [Required(ErrorMessage = "Organization name is required.")]
        [StringLength(150, MinimumLength = 2,
            ErrorMessage = "Organization name must be between 2 and 150 characters.")]
        public string? sOrgName { get; set; }


        [Url(ErrorMessage = "Enter a valid organization URL.")]
        [StringLength(250,
           ErrorMessage = "Organization URL cannot exceed 250 characters.")]
        public string? sOrgUrl { get; set; }


        // Contact Person
        [Required(ErrorMessage = "Contact person is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Contact person must be between 2 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s.]+$",
            ErrorMessage = "Contact person can contain only letters, spaces and dots.")]
        public string? sName { get; set; }

        [StringLength(100,
            ErrorMessage = "Designation cannot exceed 100 characters.")]
        public string? sDesignation { get; set; }


        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{6,20}$",
            ErrorMessage = "Mobile number must contain only digits and be between 6 and 20 digits.")]
        public string? sMobile { get; set; }


        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    ErrorMessage = "Enter a valid email address containing @ and ."
)]
        [StringLength(150,
    ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string? sEmail { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8,
           ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(
           @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
           ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
        public string? sPassword { get; set; }

        [StringLength(6, MinimumLength = 6,
            ErrorMessage = "OTP must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "OTP must contain exactly 6 digits.")]
        public string? sOTP { get; set; }

        // Database fields
        public DateTime? dRegisterDate { get; set; }

        public DateTime? dModDate { get; set; }

        public int nBit { get; set; }
    }
}