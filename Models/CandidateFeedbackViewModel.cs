namespace ERHuntCV.Models
{
    public class CandidateFeedbackViewModel
    {
        public int nID { get; set; }

        // Questions
        public string? Que1 { get; set; }
        public string? Que2 { get; set; }
        public string? Que3 { get; set; }
        public string? Que4 { get; set; }
        public string? Que5 { get; set; }

        //  answers
        public string? sQue1 { get; set; }
        public string? sQue2 { get; set; }
        public string? sQue3 { get; set; }
        public string? sQue4 { get; set; }
        public string? sQue5 { get; set; }

        // Candidate
        public int? nCandidateID { get; set; }


        // =====================================================
        // ORGANIZATION ID
        // =====================================================

        public int? nOrgID { get; set; }


        public bool? nBit { get; set; }
        public bool? nSABit { get; set; }
    }
}
