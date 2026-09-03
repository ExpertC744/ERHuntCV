using System.ComponentModel.DataAnnotations;

namespace ERHuntCV.Models
{
    public class CandidateRegisterModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please select gender.")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;
        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DOB { get; set; }
        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Please enter a valid mobile number.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number.")]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")] public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must contain only numbers.")]
        public string OTP { get; set; } = string.Empty;
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
