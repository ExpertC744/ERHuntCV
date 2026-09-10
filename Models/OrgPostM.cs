namespace ERHuntCV.Models
{
    public class OrgPostM
    {
        public int nID { get; set; }
        public string sName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }

        // Role Details
        public int nPositionID { get; set; }
        public int nRequiredTrainees { get; set; }
        public int nGenderID { get; set; }
        public string sGenderName { get; set; } = string.Empty;
        public int nMinimumQualificationID { get; set; }


        public List<Country> Countries { get; set; } = new();
        public List<State> States { get; set; } = new();
        public List<City> Cities { get; set; } = new();
        // Location
        public string sCountryID { get; set; } = string.Empty;
        public string sStateID { get; set; } = string.Empty;
        public int nCityID { get; set; }
        public string sWorkingHours { get; set; } = string.Empty;

        public string CountryID { get; set; }

        public string StateID { get; set; }

        public string CityID { get; set; }

        // Work Terms
        public int nInternshipTypeID { get; set; }
        public string sWorkingShift { get; set; } = string.Empty;
        public int nInternshipFellowshipTypeID { get; set; }

        public decimal? sTotalCharges { get; set; }
        public string sCurrency { get; set; } = string.Empty;

        // Duration
        public int nTrainingInvolvedID { get; set; }
        public int nInternshipDurationID { get; set; }

        public DateTime dStartDate { get; set; }
        public DateTime dCompletionDate { get; set; }

        // Mode
        public int nInternshipModeID { get; set; }
        public string sDivyang { get; set; } = string.Empty;
        public string sLanguageKnown { get; set; } = string.Empty;

        // Working Days
        public string? sWorkingDays { get; set; }
        public List<string> WorkingDays { get; set; } = new List<string>();

        // Facilities
        public string? sFacilities { get; set; }
        public List<string> Facilities { get; set; } = new List<string>();

        // =========================================================
        // TECHNICAL SKILL MASTER DATA
        // =========================================================

        public int nTechnicalSkillID { get; set; }

        public string sTechnicalSkillName { get; set; } = string.Empty;

        public int sTechnicalSkillRating { get; set; }


        // =========================================================
        // MEDICAL SKILL MASTER DATA
        // =========================================================

        public int nMedicalSkillID { get; set; }

        public string sMedicalSkillName { get; set; } = string.Empty;

        public int sMedicalSkillRating { get; set; }


        // =========================================================
        // NON-TECHNICAL SKILL MASTER DATA
        // =========================================================

        public int nNonTechnicalSkillID { get; set; }

        public string sNonTechnicalSkillName { get; set; } = string.Empty;

        public int sNonTechnicalSkillRating { get; set; }


        // =========================================================
        // SELECTED TECHNICAL SKILLS
        // =========================================================

        // Database value
        // Example:
        // 1,3,5

        public string? sTechnicalSkills { get; set; }


        // Form checkbox values
        public List<int> TechnicalSkillIDs { get; set; }
            = new List<int>();


        // =========================================================
        // SELECTED MEDICAL SKILLS
        // =========================================================

        // Database value
        // Example:
        // 2,4,7

        public string? sMedicalSkills { get; set; }


        // Form checkbox values
        public List<int> MedicalSkillIDs { get; set; }
            = new List<int>();


        // =========================================================
        // SELECTED NON-TECHNICAL SKILLS
        // =========================================================

        // Database value
        // Example:
        // 3,6,8

        public string? sNonTechnicalSkills { get; set; }


        // Form checkbox values
        public List<int> NonTechnicalSkillIDs { get; set; }
            = new List<int>();



        // System
        public DateTime? dRegisterDate { get; set; }
        public DateTime? dModDate { get; set; }

        public bool nBit { get; set; }
        public bool nSABit { get; set; }

        public int nOrgID { get; set; }


        // =========================================================
        // DISPLAY MASTER DATA
        // =========================================================

        public string sPositionName { get; set; } = string.Empty;

        //public string sGenderName { get; set; } = string.Empty;

        public string sMinimumQualificationName { get; set; } = string.Empty;

        public string sInternshipTypeName { get; set; } = string.Empty;

        public string sInternshipFellowshipTypeName { get; set; } = string.Empty;

        public string sTrainingInvolvedName { get; set; } = string.Empty;

        public string sInternshipDurationName { get; set; } = string.Empty;

        public string sInternshipModeName { get; set; } = string.Empty;

        public string sCityName { get; set; } = string.Empty;

        public string sStateName { get; set; } = string.Empty;

        public string sCountryName { get; set; } = string.Empty;


        public string? sCountryCode { get; set; }

        public string? sStateCode { get; set; }
    }
}
