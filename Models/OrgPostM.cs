using ERHuntCV.Models;
using System.ComponentModel.DataAnnotations;

namespace ERHuntCV.Models
{
    public class OrgPostM
    {
        public int nID { get; set; }
        public string sName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }

        // Role Details

        [Required(ErrorMessage = "Please select a position.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid position.")]
        public int nPositionID { get; set; }

        [Required(ErrorMessage = "Please enter the number of required trainees.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of required trainees must be at least 1.")]
        public int nRequiredTrainees { get; set; }
        public int MatchingTraineeCount { get; set; }

        [Required(ErrorMessage = "Please select gender.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid gender.")]
        public int nGenderID { get; set; }
        public string sGenderName { get; set; } = string.Empty;
        public int nMinimumQualificationID { get; set; }

        // Location
        //public string sCountryCode { get; set; } = string.Empty;
        //public string sStateCode { get; set; } = string.Empty;
        //public int nCityID { get; set; }

        [Required(ErrorMessage = "Please select a country.")]
        public string sCountryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a state.")]
        public string sStateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a city.")]
        public string nCityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter working hours.")]
        public string sWorkingHours { get; set; } = string.Empty;

        // Work Terms

        [Required(ErrorMessage = "Please select internship type.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid internship type.")]
        public int nInternshipTypeID { get; set; }

        [Required(ErrorMessage = "Please select working shift.")]
        public string sWorkingShift { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select internship fellowship type.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid fellowship type.")]
        public int nInternshipFellowshipTypeID { get; set; }

        [Required(ErrorMessage = "Please enter charges.")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid charge amount.")]
        public decimal? sTotalCharges { get; set; }

        [Required(ErrorMessage = "Please select currency.")]
        public string sCurrency { get; set; } = string.Empty;

        // Duration
        [Required(ErrorMessage = "Please select training involvement.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid training option.")]
        public int nTrainingInvolvedID { get; set; }

        [Required(ErrorMessage = "Please select internship duration.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid internship duration.")]
        public int nInternshipDurationID { get; set; }

        public DateTime? dStartDate { get; set; }
        public DateTime dCompletionDate { get; set; }

        // Mode

        [Required(ErrorMessage = "Please select internship mode.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid internship mode.")]
        public int nInternshipModeID { get; set; }

        [Required(ErrorMessage = "Please specify Divyang option.")]
        public string sDivyang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter known languages.")]
        public string sLanguageKnown { get; set; } = string.Empty;

        // Working Days
        [Required(ErrorMessage = "Please select at least one working day.")]
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


        public string sOrgName { get; set; } = string.Empty;
        public string sOrgCode { get; set; } = string.Empty;

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


        //public string sStateName { get; set; } = string.Empty;

        //public string sCountryName { get; set; } = string.Empty;

        public int AppliedTraineeCount { get; set; }

        public bool IsApplied { get; set; }

        //for matching jobs 
        public string? OrganizationName { get; set; }
        public string? InternshipFellowshipTypeName { get; set; }





    }
}