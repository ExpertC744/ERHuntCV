namespace ERHuntCV.Models
{
    public class CandidateProfileViewModel
    {
        public CandidateProfileModel Profile { get; set; }
            = new CandidateProfileModel();

        public List<DropdownModel> Divisions { get; set; } = new();
        public List<DropdownModel> Streams { get; set; } = new();
        public List<DropdownModel> GraduationStatuses { get; set; } = new();

        public List<DropdownModel> InternshipFellowshipType { get; set; } = new();
        public List<DropdownModel> InternshipTitles { get; set; } = new();
        public List<DropdownModel> InternshipDurations { get; set; } = new();
        public List<DropdownModel> InternshipStatuses { get; set; } = new();
        public List<DropdownModel> Relationships { get; set; } = new();

        // ADD THESE
        public List<Country> Countries { get; set; } = new();
        public List<State> States { get; set; } = new();
        public List<City> Cities { get; set; } = new();
    }
}
