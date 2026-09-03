using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HuntCV_Portal.Models
{
    public class CandidateRegisterM
    {
        public int nID { get; set; }


        // ==============================
        // FIRST NAME
        // ==============================

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(?:\s[a-zA-Z]+)*$",
            ErrorMessage = "First name can contain only letters and spaces.")]
        public string sFName { get; set; } = string.Empty;


        // ==============================
        // LAST NAME
        // ==============================

        [StringLength(50,
            ErrorMessage = "Last name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(?:\s[a-zA-Z]+)*$",
            ErrorMessage = "Last name can contain only letters and spaces.")]
        public string? sLName { get; set; }


        // ==============================
        // GENDER
        // ==============================

        [Required(ErrorMessage = "Please select your gender.")]
        [Range(1, 3,
            ErrorMessage = "Please select a valid gender.")]
        public int? nGender { get; set; }


        // ==============================
        // DATE OF BIRTH
        // ==============================

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }


        // ==============================
        // MOBILE
        // ==============================

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{6,20}$",
            ErrorMessage = "Mobile number must contain only digits and be between 6 and 20 digits.")]
        public string sMobile { get; set; } = string.Empty;


        // ==============================
        // EMAIL
        // ==============================

        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Enter a valid email address containing @ and .")]
        [StringLength(150,
            ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string sEmail { get; set; } = string.Empty;


        // ==============================
        // OTP
        // ==============================

        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(6, MinimumLength = 6,
            ErrorMessage = "OTP must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "OTP must contain exactly 6 digits.")]
        public string? sOTP { get; set; }


        // ==============================
        // PROFILE IMAGE NAME
        // ==============================

        public string? sProfileImage { get; set; }


        // ==============================
        // PASSWORD
        // ==============================

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string sPassword { get; set; } = string.Empty;


        // ==============================
        // CAPTCHA
        // ==============================

        [Required(ErrorMessage = "CAPTCHA is required.")]
        [StringLength(20,
            ErrorMessage = "CAPTCHA cannot exceed 20 characters.")]
        public string? sCaptcha { get; set; }


        // ==============================
        // PROFILE IMAGE
        // ==============================

        public IFormFile? ProfileImageFile { get; set; }
    }
}