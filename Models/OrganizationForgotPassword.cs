using System.ComponentModel.DataAnnotations;

namespace ERHuntCV.Models
{
    public class OrganizationForgotPassword
    {
        [Required(ErrorMessage = "Please enter your registered email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string sEmail { get; set; } = "";
    }
}
