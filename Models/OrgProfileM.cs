using System.ComponentModel.DataAnnotations;

namespace ERHuntCV.Models
{
    public class OrgProfileM
    {
        public int nID { get; set; }

        public int nOrgID { get; set; }

        public string sOrganizationName { get; set; } = string.Empty;

        public string sDesignation { get; set; } = string.Empty;

        public string sMobile { get; set; } = string.Empty;

        public string sOrganizationEmail { get; set; } = string.Empty;

        public DateTime? dDateOfBirth { get; set; }

        public string? sCompanyLogo { get; set; }

        public string sCompanyAddress { get; set; } = string.Empty;

        public int nEstablishmentYear { get; set; }

        public string? sGSTNo { get; set; }

        public string? sCINNo { get; set; }

        public string nEmployeeStrength { get; set; } = string.Empty;

        public DateTime dCreatedDate { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public bool nBit { get; set; }

        public bool? nSABit { get; set; }
    }
}
