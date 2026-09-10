namespace ERHuntCV.Models
{
    public class Country
    {
        public string Id { get; set; }

        public string Iso2 { get; set; }

        public string Name { get; set; }
    }


    public class State
    {
        public string Id { get; set; }

        public string Iso2 { get; set; }

        public string Name { get; set; }

        public string CountryCode { get; set; }

        // ADD THIS
        public string StateCode { get; set; }
    }


    public class City
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CountryCode { get; set; }

        public string StateCode { get; set; }
    }
}
