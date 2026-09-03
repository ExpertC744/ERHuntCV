namespace ERHuntCV.Models
{
    public class SAOrgFeedbackDetailsM
    {
        public int nID { get; set; }

        public string sOrgName { get; set; }
        public string sOrgLogo { get; set; }

        public int FeedbackID { get; set; }
        public int nSAID { get; set; }

        public string sQue1 { get; set; }
        public string sQue2 { get; set; }
        public string sQue3 { get; set; }
        public string sQue4 { get; set; }
        public string sQue5 { get; set; }

        public string sAns1 { get; set; }
        public string sAns2 { get; set; }
        public string sAns3 { get; set; }
        public string sAns4 { get; set; }
        public string sAns5 { get; set; }

        public bool nSABit { get; set; }

        public DateTime? RegDate { get; set; }
        public DateTime? ModDate { get; set; }
    }
}
