namespace ERHuntCV.Models
{
    public class SACandidateFeedbackDetailsM
    {
        // Candidate Register
        public int nID { get; set; }

        public string? sFName { get; set; }

        public string? sLName { get; set; }

        public string? sProfileImage { get; set; }


        // Candidate Feedback
        public int nAdminID { get; set; }

        public int sQue1 { get; set; }

        public int sQue2 { get; set; }

        public int sQue3 { get; set; }

        public int sQue4 { get; set; }

        public string? sQue5 { get; set; }


        // Status
        public bool nSABit { get; set; }


        // Dates
        public DateTime? RegDate { get; set; }

        public DateTime? ModDate { get; set; }
    }
}
