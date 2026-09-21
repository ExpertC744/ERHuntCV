using System.ComponentModel.DataAnnotations;

namespace ERHuntCV.Models
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
        public string sEmail { get; set; } = string.Empty;

        public string? sMobile { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string sPassword { get; set; } = string.Empty;

        public DateTime RegDate { get; set; }

        public DateTime? ModDate { get; set; }

        public bool nBit { get; set; }

        public string? sRole { get; set; }
    }
}