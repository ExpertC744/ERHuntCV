namespace ERHuntCV.Models
{
    public class CandidateProfileModel
    {
        public int nID { get; set; }
        public int CandidateID { get; set; }

        // Address
        public string? CountryID { get; set; }
        public string? StateID { get; set; }
        public string? CityID { get; set; }
        public string? Pincode { get; set; }

        // Education

        public int? SSC_YEAR { get; set; }
        public int? SSC_DIVISION { get; set; }

        public int? HSC_DIPLOMA_YEAR { get; set; }
        public int? HSC_DIPLOMA_DIVISION { get; set; }

        public int? Graduation_Year { get; set; }
        public int? Graduation_Division { get; set; }
        public int? Graduation_Stream { get; set; }
        public string? Other_Stream { get; set; }

        public int? PG_Year { get; set; }
        public int? PG_Division { get; set; }
        public int? PG_Stream { get; set; }
        public string? Other_Specialization { get; set; }

        public int? PhD_Year { get; set; }
        public int? PhD_Status { get; set; }
        public string? PhD_Topic { get; set; }
        public string? Previous_PhD_Topic_Year { get; set; }

        //Internship_FellowshipType
        public int? Internship_FellowshipType { get; set; }
        public string? Preferred_Country { get; set; }
        public string? Preferred_State { get; set; }
        public string? Preferred_City { get; set; }

        // DOCUMENTS
        public string? sResume { get; set; }
        public string? sPhoto { get; set; }
        public string? sSignature { get; set; }
        public string? sDivyang { get; set; }

        // HOBBIES
        public string? sHobbies { get; set; }

        // INTERNSHIP DETAILS
        public string? sOrgName1 { get; set; }
        public int? sOrgIntTitle1 { get; set; }
        public int? sOrgIntDuration1 { get; set; }
        public int? sOrgIntStatus1 { get; set; }
        public string? sOrgName2 { get; set; }
        public int? sOrgIntTitle2 { get; set; }
        public int? sOrgIntDuration2 { get; set; }
        public int? sOrgIntStatus2 { get; set; }
        public string? sOrgName3 { get; set; }
        public int? sOrgIntTitle3 { get; set; }
        public int? sOrgIntDuration3 { get; set; }
        public int? sOrgIntStatus3 { get; set; }
        public string? sOrgName4 { get; set; }
        public int? sOrgIntTitle4 { get; set; }
        public int? sOrgIntDuration4 { get; set; }
        public int? sOrgIntStatus4 { get; set; }

        // Languages Known
        public string? sLanguage1 { get; set; }
        public string? sLanguage2 { get; set; }
        public string? sLanguage3 { get; set; }
        public string? sLanguage4 { get; set; }
        public string? sLanguage5 { get; set; }
        public string? sLanguage6 { get; set; }
        public string? sLanguage7 { get; set; }
        public string? sLanguage8 { get; set; }
        public int? sLanguageStar1 { get; set; }
        public int? sLanguageStar2 { get; set; }
        public int? sLanguageStar3 { get; set; }
        public int? sLanguageStar4 { get; set; }
        public int? sLanguageStar5 { get; set; }
        public int? sLanguageStar6 { get; set; }
        public int? sLanguageStar7 { get; set; }
        public int? sLanguageStar8 { get; set; }

        // Reference
        public string? sRefName1 { get; set; }
        public string? sRefName2 { get; set; }
        public int? sRefRelationName1 { get; set; }
        public int? sRefRelationName2 { get; set; }
        public string? sLocation1 { get; set; }
        public string? sLocation2 { get; set; }
        public string? sMobile1 { get; set; }
        public string? sMobile2 { get; set; }

        // Achievements / Certification
        public string? Achievements_Certification1 { get; set; }
        public string? Achievements_Certification2 { get; set; }
        public string? Achievements_Certification3 { get; set; }

        //Add Links
        public string? GitHub { get; set; }
        public string? Linkedin { get; set; }

        // Objective
        public string? Objective { get; set; }
        public int? Resume_Profile { get; set; }

        //Skills
        public string? sMedicalSkillIDs { get; set; }
        public string? sMedicalSkillStarIDs { get; set; }
        public string? sTechnicalSkillIDs { get; set; }
        public string? sTechnicalSkillStarIDs { get; set; }
        public string? sNonTechnicalSkillIDs { get; set; }
        public string? sNonTechnicalSkillStarIDs { get; set; }
    }
}
