
namespace ERHuntCV.Models
{
    public class SAChartData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalCount { get; set; }
    }

    public class SAChartsViewModel
    {
        public List<SAChartData> CandidateData { get; set; }
            = new List<SAChartData>();

        public List<SAChartData> OrganizationData { get; set; }
            = new List<SAChartData>();

        public List<SAChartData> InternshipPostData { get; set; }
            = new List<SAChartData>();
    }
}

