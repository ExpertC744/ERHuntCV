namespace ERHuntCV.Models
{
    public class OrganizationUserM
    {
        public int nID { get; set; }

        public string? sOrgName { get; set; }

        public string? sOrgUrl { get; set; }

        public string? sName { get; set; }

        public string? sDesignation { get; set; }

        public string? sMobile { get; set; }

        public string? sEmail { get; set; }

        public int? nCollegeCode { get; set; }

        public int? nCollegeName { get; set; }

        public string? sPassword { get; set; }

        public DateTime? RegDate { get; set; }

        public DateTime? ModDate { get; set; }

        public bool nBit { get; set; }

        public bool nSABit { get; set; }
    }
}
