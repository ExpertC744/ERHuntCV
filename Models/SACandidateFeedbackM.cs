namespace ERHuntCV.Models
{
    public class SACandidateFeedbackM
    {
        public int nID { get; set; }

        public string? Que1 { get; set; }
        public string? Ans1 { get; set; }

        public string? Que2 { get; set; }
        public string? Ans2 { get; set; }

        public string? Que3 { get; set; }
        public string? Ans3 { get; set; }

        public string? Que4 { get; set; }
        public string? Ans4 { get; set; }

        public string? Que5 { get; set; }
        public string? Ans5 { get; set; }

        public bool? nBit { get; set; }
        public bool? nSABit { get; set; }

        public int nSAID { get; set; }

        public DateTime? dRegDate { get; set; }
        public DateTime? dModDate { get; set; }
    }
}
