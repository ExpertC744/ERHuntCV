using ERHuntCV.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace ERHuntCV.Repositories
{
    public class CandidateProfileRepository
    {
        private readonly IConfiguration _configuration;

        public CandidateProfileRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("DefaultConnection is missing from appsettings.json.");
            }

            return new SqlConnection(connectionString);
        }

        // =========================================================
        // ENSURE PROFILE EXISTS
        // =========================================================

        private void EnsureProfileExists(SqlConnection con, int candidateId)
        {
            string sql = @"
        IF NOT EXISTS
        (
            SELECT 1
            FROM tblCandidateProfile
            WHERE CandidateID = @CandidateID
        )
        BEGIN
            INSERT INTO tblCandidateProfile
            (
                CandidateID,
                RegDate,
                ModDate,
                nBit,
                nSABit
            )
            VALUES
            (
                @CandidateID,
                GETDATE(),
                GETDATE(),
                1,
                1
            )
        END";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = candidateId;

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // GET PROFILE
        // =========================================================

        public CandidateProfileModel? GetProfile(int candidateId)
        {
            using SqlConnection con = GetConnection();

            string sql = @"SELECT * FROM tblCandidateProfile WHERE CandidateID = @CandidateID";
            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int).Value = candidateId;

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new CandidateProfileModel
            {
                nID = GetNullableInt(reader, "nID") ?? 0,
                CandidateID = GetNullableInt(reader, "CandidateID") ?? 0,

                // ADDRESS
                CountryID = GetNullableString(reader, "CountryID"),
                StateID = GetNullableString(reader, "StateID"),
                CityID = GetNullableString(reader, "CityID"),
                Pincode = GetNullableString(reader, "Pincode"),

                // EDUCATION

                SSC_YEAR = GetNullableInt(reader, "SSC_YEAR"),

                SSC_DIVISION = GetNullableInt(reader, "SSC_DIVISION"),

                HSC_DIPLOMA_YEAR = GetNullableInt(reader, "HSC_DIPLOMA_YEAR"),

                HSC_DIPLOMA_DIVISION = GetNullableInt(reader, "HSC_DIPLOMA_DIVISION"),

                Graduation_Year = GetNullableInt(reader, "Graduation_Year"),

                Graduation_Division = GetNullableInt(reader, "Graduation_Division"),

                Graduation_Stream = GetNullableInt(reader, "Graduation_Stream"),

                Other_Stream = GetNullableString(reader, "Other_Stream"),

                PG_Year = GetNullableInt(reader, "PG_Year"),

                PG_Division = GetNullableInt(reader, "PG_Division"),

                PG_Stream = GetNullableInt(reader, "PG_Stream"),

                Other_Specialization = GetNullableString(reader, "Other_Specialization"),

                PhD_Year = GetNullableInt(reader, "PhD_Year"),

                PhD_Status = GetNullableInt(reader, "PhD_Status"),

                PhD_Topic = GetNullableString(reader, "PhD_Topic"),

                Previous_PhD_Topic_Year = GetNullableString(reader, "Previous_PhD_Topic_Year"),


                // INTERNSHIP PREFERENCE

                Internship_FellowshipType = GetNullableInt(reader, "Internship_FellowshipType"),

                Preferred_Country = GetNullableString(reader, "Preferred_Country"),

                Preferred_State = GetNullableString(reader, "Preferred_State"),

                Preferred_City = GetNullableString(reader, "Preferred_City"),


                // DOCUMENTS
                sResume = GetNullableString(reader, "sResume"),
                sPhoto = GetNullableString(reader, "sPhoto"),
                sSignature = GetNullableString(reader, "sSignature"),
                sDivyang = GetNullableString(reader, "sDivyang"),
                sHobbies = GetNullableString(reader, "sHobbies"),

                // INTERNSHIP 1
                sOrgName1 = GetNullableString(reader, "sOrgName1"),
                sOrgIntTitle1 = GetNullableInt(reader, "sOrgIntTitle1"),
                sOrgIntDuration1 = GetNullableInt(reader, "sOrgIntDuration1"),
                sOrgIntStatus1 = GetNullableInt(reader, "sOrgIntStatus1"),

                // INTERNSHIP 2
                sOrgName2 = GetNullableString(reader, "sOrgName2"),
                sOrgIntTitle2 = GetNullableInt(reader, "sOrgIntTitle2"),
                sOrgIntDuration2 = GetNullableInt(reader, "sOrgIntDuration2"),
                sOrgIntStatus2 = GetNullableInt(reader, "sOrgIntStatus2"),

                // INTERNSHIP 3
                sOrgName3 = GetNullableString(reader, "sOrgName3"),
                sOrgIntTitle3 = GetNullableInt(reader, "sOrgIntTitle3"),
                sOrgIntDuration3 = GetNullableInt(reader, "sOrgIntDuration3"),
                sOrgIntStatus3 = GetNullableInt(reader, "sOrgIntStatus3"),

                // INTERNSHIP 4
                sOrgName4 = GetNullableString(reader, "sOrgName4"),
                sOrgIntTitle4 = GetNullableInt(reader, "sOrgIntTitle4"),
                sOrgIntDuration4 = GetNullableInt(reader, "sOrgIntDuration4"),
                sOrgIntStatus4 = GetNullableInt(reader, "sOrgIntStatus4"),


                // LANGUAGES
                sLanguage1 = GetNullableString(reader, "sLanguage1"),
                sLanguage2 = GetNullableString(reader, "sLanguage2"),
                sLanguage3 = GetNullableString(reader, "sLanguage3"),
                sLanguage4 = GetNullableString(reader, "sLanguage4"),
                sLanguage5 = GetNullableString(reader, "sLanguage5"),
                sLanguage6 = GetNullableString(reader, "sLanguage6"),
                sLanguage7 = GetNullableString(reader, "sLanguage7"),
                sLanguage8 = GetNullableString(reader, "sLanguage8"),
                sLanguageStar1 = GetNullableInt(reader, "sLanguageStar1"),
                sLanguageStar2 = GetNullableInt(reader, "sLanguageStar2"),
                sLanguageStar3 = GetNullableInt(reader, "sLanguageStar3"),
                sLanguageStar4 = GetNullableInt(reader, "sLanguageStar4"),
                sLanguageStar5 = GetNullableInt(reader, "sLanguageStar5"),
                sLanguageStar6 = GetNullableInt(reader, "sLanguageStar6"),
                sLanguageStar7 = GetNullableInt(reader, "sLanguageStar7"),
                sLanguageStar8 = GetNullableInt(reader, "sLanguageStar8"),


                // REFERENCES

                sRefName1 =
                    GetNullableString(reader, "sRefName1"),

                sRefName2 =
                    GetNullableString(reader, "sRefName2"),

                sRefRelationName1 =
                    GetNullableInt(
                        reader,
                        "sRefRelationName1"
                    ),

                sRefRelationName2 =
                    GetNullableInt(
                        reader,
                        "sRefRelationName2"
                    ),

                sLocation1 =
                    GetNullableString(reader, "sLocation1"),

                sLocation2 =
                    GetNullableString(reader, "sLocation2"),

                sMobile1 =
                    GetNullableString(reader, "sMobile1"),

                sMobile2 =
                    GetNullableString(reader, "sMobile2"),


                // ACHIEVEMENTS

                Achievements_Certification1 =
                    GetNullableString(
                        reader,
                        "Achievements_Certification1"
                    ),

                Achievements_Certification2 =
                    GetNullableString(
                        reader,
                        "Achievements_Certification2"
                    ),

                Achievements_Certification3 =
                    GetNullableString(
                        reader,
                        "Achievements_Certification3"
                    ),


                // LINKS

                GitHub =
                    GetNullableString(reader, "GitHub"),

                Linkedin =
                    GetNullableString(reader, "Linkedin"),


                // OBJECTIVE

                Objective =
                    GetNullableString(reader, "Objective"),

                Resume_Profile =
                    GetNullableInt(reader, "Resume_Profile"),


                // SKILLS

                sMedicalSkillIDs =
                    GetNullableString(
                        reader,
                        "sMedicalSkillIDs"
                    ),

                sMedicalSkillStarIDs =
                    GetNullableString(
                        reader,
                        "sMedicalSkillStarIDs"
                    ),

                sTechnicalSkillIDs =
                    GetNullableString(
                        reader,
                        "sTechnicalSkillIDs"
                    ),

                sTechnicalSkillStarIDs =
                    GetNullableString(
                        reader,
                        "sTechnicalSkillStarIDs"
                    ),

                sNonTechnicalSkillIDs =
                    GetNullableString(
                        reader,
                        "sNonTechnicalSkillIDs"
                    ),

                sNonTechnicalSkillStarIDs =
                    GetNullableString(
                        reader,
                        "sNonTechnicalSkillStarIDs"
                    )
            };
        }

        // =========================================================
        // UPDATE ADDRESS
        // =========================================================

        public void UpdateAddress(
            int candidateId,
            string? countryId,
            string? stateId,
            string? cityId,
            string? pincode)
        {
            using SqlConnection con = GetConnection();

            con.Open();

            EnsureProfileExists(con, candidateId);

            string sql = @"
        UPDATE tblCandidateProfile
        SET
            CountryID = @CountryID,
            StateID = @StateID,
            CityID = @CityID,
            Pincode = @Pincode,
            ModDate = GETDATE()
        WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add(
                "@CandidateID",
                SqlDbType.Int
            ).Value = candidateId;

            cmd.Parameters.Add(
                "@CountryID",
                SqlDbType.NVarChar,
                100
            ).Value = (object?)countryId ?? DBNull.Value;

            cmd.Parameters.Add(
                "@StateID",
                SqlDbType.NVarChar,
                100
            ).Value = (object?)stateId ?? DBNull.Value;

            cmd.Parameters.Add(
                "@CityID",
                SqlDbType.NVarChar,
                100
            ).Value = (object?)cityId ?? DBNull.Value;

            cmd.Parameters.Add(
                "@Pincode",
                SqlDbType.NVarChar,
                20
            ).Value = (object?)pincode ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }

        // =========================================================
        // UPDATE EDUCATION
        // =========================================================

        public void UpdateEducation(CandidateProfileModel model)
        {
            using SqlConnection con = GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"

        UPDATE tblCandidateProfile
        SET

            SSC_YEAR = @SSC_YEAR,
            SSC_DIVISION = @SSC_DIVISION,

            HSC_DIPLOMA_YEAR = @HSC_DIPLOMA_YEAR,
            HSC_DIPLOMA_DIVISION = @HSC_DIPLOMA_DIVISION,

            Graduation_Year = @Graduation_Year,
            Graduation_Division = @Graduation_Division,
            Graduation_Stream = @Graduation_Stream,
            Other_Stream = @Other_Stream,

            PG_Year = @PG_Year,
            PG_Division = @PG_Division,
            PG_Stream = @PG_Stream,
            Other_Specialization = @Other_Specialization,

            PhD_Year = @PhD_Year,
            PhD_Status = @PhD_Status,
            PhD_Topic = @PhD_Topic,
            Previous_PhD_Topic_Year = @Previous_PhD_Topic_Year,

            ModDate = GETDATE()

        WHERE CandidateID = @CandidateID";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            cmd.Parameters.Add("@SSC_YEAR", SqlDbType.Int)
                .Value = (object?)model.SSC_YEAR ?? DBNull.Value;

            cmd.Parameters.Add("@SSC_DIVISION", SqlDbType.Int)
                .Value = (object?)model.SSC_DIVISION ?? DBNull.Value;

            cmd.Parameters.Add("@HSC_DIPLOMA_YEAR", SqlDbType.Int)
                .Value = (object?)model.HSC_DIPLOMA_YEAR ?? DBNull.Value;

            cmd.Parameters.Add("@HSC_DIPLOMA_DIVISION", SqlDbType.Int)
                .Value = (object?)model.HSC_DIPLOMA_DIVISION ?? DBNull.Value;

            cmd.Parameters.Add("@Graduation_Year", SqlDbType.Int)
                .Value = (object?)model.Graduation_Year ?? DBNull.Value;

            cmd.Parameters.Add("@Graduation_Division", SqlDbType.Int)
                .Value = (object?)model.Graduation_Division ?? DBNull.Value;

            cmd.Parameters.Add("@Graduation_Stream", SqlDbType.Int)
                .Value = (object?)model.Graduation_Stream ?? DBNull.Value;

            cmd.Parameters.Add("@Other_Stream", SqlDbType.NVarChar, 100)
                .Value = (object?)model.Other_Stream ?? DBNull.Value;

            cmd.Parameters.Add("@PG_Year", SqlDbType.Int)
                .Value = (object?)model.PG_Year ?? DBNull.Value;

            cmd.Parameters.Add("@PG_Division", SqlDbType.Int)
                .Value = (object?)model.PG_Division ?? DBNull.Value;

            cmd.Parameters.Add("@PG_Stream", SqlDbType.Int)
                .Value = (object?)model.PG_Stream ?? DBNull.Value;

            cmd.Parameters.Add("@Other_Specialization", SqlDbType.NVarChar, 100)
                .Value = (object?)model.Other_Specialization ?? DBNull.Value;

            cmd.Parameters.Add("@PhD_Year", SqlDbType.Int)
                .Value = (object?)model.PhD_Year ?? DBNull.Value;

            cmd.Parameters.Add("@PhD_Status", SqlDbType.Int)
                .Value = (object?)model.PhD_Status ?? DBNull.Value;

            cmd.Parameters.Add("@PhD_Topic", SqlDbType.NVarChar, 300)
                .Value = (object?)model.PhD_Topic ?? DBNull.Value;

            cmd.Parameters.Add("@Previous_PhD_Topic_Year", SqlDbType.NVarChar, 300)
                .Value = (object?)model.Previous_PhD_Topic_Year ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE INTERNSHIP / FELLOWSHIP PREFERENCE
        // =========================================================

        public void UpdateInternshipPreference(CandidateProfileModel model)
        {
            using SqlConnection con = GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
        UPDATE tblCandidateProfile
        SET
            Internship_FellowshipType = @Internship_FellowshipType,
            Preferred_Country = @Preferred_Country,
            Preferred_State = @Preferred_State,
            Preferred_City = @Preferred_City,
            ModDate = GETDATE()
        WHERE CandidateID = @CandidateID";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            cmd.Parameters.Add(
                "@Internship_FellowshipType",
                SqlDbType.Int
            ).Value = (object?)model.Internship_FellowshipType ?? DBNull.Value;

            cmd.Parameters.Add(
                "@Preferred_Country",
                SqlDbType.NVarChar,
                100
            ).Value = (object?)model.Preferred_Country ?? DBNull.Value;

            cmd.Parameters.Add(
                "@Preferred_State",
                SqlDbType.NVarChar,
                100
            ).Value = (object?)model.Preferred_State ?? DBNull.Value;

            cmd.Parameters.Add(
                "@Preferred_City",
                SqlDbType.NVarChar,
                100
            ).Value = (object?)model.Preferred_City ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE INTERNSHIP DETAILS
        // =========================================================

        public void UpdateInternshipDetails(CandidateProfileModel model)
        {
            using SqlConnection con = GetConnection();
            con.Open();
            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
UPDATE tblCandidateProfile
SET

    sOrgName1 = @sOrgName1,
    sOrgIntTitle1 = @sOrgIntTitle1,
    sOrgIntDuration1 = @sOrgIntDuration1,
    sOrgIntStatus1 = @sOrgIntStatus1,

    sOrgName2 = @sOrgName2,
    sOrgIntTitle2 = @sOrgIntTitle2,
    sOrgIntDuration2 = @sOrgIntDuration2,
    sOrgIntStatus2 = @sOrgIntStatus2,

    sOrgName3 = @sOrgName3,
    sOrgIntTitle3 = @sOrgIntTitle3,
    sOrgIntDuration3 = @sOrgIntDuration3,
    sOrgIntStatus3 = @sOrgIntStatus3,

    sOrgName4 = @sOrgName4,
    sOrgIntTitle4 = @sOrgIntTitle4,
    sOrgIntDuration4 = @sOrgIntDuration4,
    sOrgIntStatus4 = @sOrgIntStatus4,

    ModDate = GETDATE()

WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(cmd, "@sOrgName1", model.sOrgName1, 300);
            AddNullableInt(cmd, "@sOrgIntTitle1", model.sOrgIntTitle1);
            AddNullableInt(cmd, "@sOrgIntDuration1", model.sOrgIntDuration1);
            AddNullableInt(cmd, "@sOrgIntStatus1", model.sOrgIntStatus1);

            AddNullableString(cmd, "@sOrgName2", model.sOrgName2, 300);
            AddNullableInt(cmd, "@sOrgIntTitle2", model.sOrgIntTitle2);
            AddNullableInt(cmd, "@sOrgIntDuration2", model.sOrgIntDuration2);
            AddNullableInt(cmd, "@sOrgIntStatus2", model.sOrgIntStatus2);

            AddNullableString(cmd, "@sOrgName3", model.sOrgName3, 300);
            AddNullableInt(cmd, "@sOrgIntTitle3", model.sOrgIntTitle3);
            AddNullableInt(cmd, "@sOrgIntDuration3", model.sOrgIntDuration3);
            AddNullableInt(cmd, "@sOrgIntStatus3", model.sOrgIntStatus3);

            AddNullableString(cmd, "@sOrgName4", model.sOrgName4, 300);
            AddNullableInt(cmd, "@sOrgIntTitle4", model.sOrgIntTitle4);
            AddNullableInt(cmd, "@sOrgIntDuration4", model.sOrgIntDuration4);
            AddNullableInt(cmd, "@sOrgIntStatus4", model.sOrgIntStatus4);

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE LANGUAGES
        // =========================================================

        public void UpdateLanguages(
            CandidateProfileModel model)
        {
            using SqlConnection con =
                GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
UPDATE tblCandidateProfile
SET

    sLanguage1 = @sLanguage1,
    sLanguage2 = @sLanguage2,
    sLanguage3 = @sLanguage3,
    sLanguage4 = @sLanguage4,
    sLanguage5 = @sLanguage5,
    sLanguage6 = @sLanguage6,
    sLanguage7 = @sLanguage7,
    sLanguage8 = @sLanguage8,

    sLanguageStar1 = @sLanguageStar1,
    sLanguageStar2 = @sLanguageStar2,
    sLanguageStar3 = @sLanguageStar3,
    sLanguageStar4 = @sLanguageStar4,
    sLanguageStar5 = @sLanguageStar5,
    sLanguageStar6 = @sLanguageStar6,
    sLanguageStar7 = @sLanguageStar7,
    sLanguageStar8 = @sLanguageStar8,

    ModDate = GETDATE()

WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(cmd, "@sLanguage1", model.sLanguage1, 100);
            AddNullableString(cmd, "@sLanguage2", model.sLanguage2, 100);
            AddNullableString(cmd, "@sLanguage3", model.sLanguage3, 100);
            AddNullableString(cmd, "@sLanguage4", model.sLanguage4, 100);
            AddNullableString(cmd, "@sLanguage5", model.sLanguage5, 100);
            AddNullableString(cmd, "@sLanguage6", model.sLanguage6, 100);
            AddNullableString(cmd, "@sLanguage7", model.sLanguage7, 100);
            AddNullableString(cmd, "@sLanguage8", model.sLanguage8, 100);

            AddNullableInt(cmd, "@sLanguageStar1", model.sLanguageStar1);
            AddNullableInt(cmd, "@sLanguageStar2", model.sLanguageStar2);
            AddNullableInt(cmd, "@sLanguageStar3", model.sLanguageStar3);
            AddNullableInt(cmd, "@sLanguageStar4", model.sLanguageStar4);
            AddNullableInt(cmd, "@sLanguageStar5", model.sLanguageStar5);
            AddNullableInt(cmd, "@sLanguageStar6", model.sLanguageStar6);
            AddNullableInt(cmd, "@sLanguageStar7", model.sLanguageStar7);
            AddNullableInt(cmd, "@sLanguageStar8", model.sLanguageStar8);

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE REFERENCES
        // =========================================================

        public void UpdateReferences(
            CandidateProfileModel model)
        {
            using SqlConnection con =
                GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
UPDATE tblCandidateProfile
SET

    sRefName1 = @sRefName1,
    sRefName2 = @sRefName2,

    sRefRelationName1 = @sRefRelationName1,
    sRefRelationName2 = @sRefRelationName2,

    sLocation1 = @sLocation1,
    sLocation2 = @sLocation2,

    sMobile1 = @sMobile1,
    sMobile2 = @sMobile2,

    ModDate = GETDATE()

WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(cmd, "@sRefName1", model.sRefName1, 200);
            AddNullableString(cmd, "@sRefName2", model.sRefName2, 200);

            AddNullableInt(cmd,
                "@sRefRelationName1",
                model.sRefRelationName1);

            AddNullableInt(cmd,
                "@sRefRelationName2",
                model.sRefRelationName2);

            AddNullableString(cmd,
                "@sLocation1",
                model.sLocation1,
                300);

            AddNullableString(cmd,
                "@sLocation2",
                model.sLocation2,
                300);

            AddNullableString(cmd,
                "@sMobile1",
                model.sMobile1,
                20);

            AddNullableString(cmd,
                "@sMobile2",
                model.sMobile2,
                20);

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE ACHIEVEMENTS
        // =========================================================

        public void UpdateAchievements(
            CandidateProfileModel model)
        {
            using SqlConnection con =
                GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
UPDATE tblCandidateProfile
SET

    Achievements_Certification1 =
        @Achievements_Certification1,

    Achievements_Certification2 =
        @Achievements_Certification2,

    Achievements_Certification3 =
        @Achievements_Certification3,

    ModDate = GETDATE()

WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(
                cmd,
                "@Achievements_Certification1",
                model.Achievements_Certification1,
                500
            );

            AddNullableString(
                cmd,
                "@Achievements_Certification2",
                model.Achievements_Certification2,
                500
            );

            AddNullableString(
                cmd,
                "@Achievements_Certification3",
                model.Achievements_Certification3,
                500
            );

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE LINKS
        // =========================================================

        public void UpdateLinks(
            CandidateProfileModel model)
        {
            using SqlConnection con =
                GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
UPDATE tblCandidateProfile
SET

    GitHub = @GitHub,
    Linkedin = @Linkedin,

    ModDate = GETDATE()

WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(
                cmd,
                "@GitHub",
                model.GitHub,
                500
            );

            AddNullableString(
                cmd,
                "@Linkedin",
                model.Linkedin,
                500
            );

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE OBJECTIVE
        // =========================================================

        public void UpdateObjective(CandidateProfileModel model)
        {
            using SqlConnection con = GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
        UPDATE tblCandidateProfile
        SET
            Objective = @Objective,
            ModDate = GETDATE()
        WHERE CandidateID = @CandidateID";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            cmd.Parameters.Add("@Objective", SqlDbType.NVarChar, 1000)
                .Value = (object?)model.Objective ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE SKILLS
        // =========================================================

        public void UpdateSkills(
            CandidateProfileModel model)
        {
            using SqlConnection con =
                GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
UPDATE tblCandidateProfile
SET

    sMedicalSkillIDs =
        @sMedicalSkillIDs,

    sMedicalSkillStarIDs =
        @sMedicalSkillStarIDs,

    sTechnicalSkillIDs =
        @sTechnicalSkillIDs,

    sTechnicalSkillStarIDs =
        @sTechnicalSkillStarIDs,

    sNonTechnicalSkillIDs =
        @sNonTechnicalSkillIDs,

    sNonTechnicalSkillStarIDs =
        @sNonTechnicalSkillStarIDs,

    ModDate = GETDATE()

WHERE CandidateID = @CandidateID";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(
                cmd,
                "@sMedicalSkillIDs",
                model.sMedicalSkillIDs,
                500
            );

            AddNullableString(
                cmd,
                "@sMedicalSkillStarIDs",
                model.sMedicalSkillStarIDs,
                100
            );

            AddNullableString(
                cmd,
                "@sTechnicalSkillIDs",
                model.sTechnicalSkillIDs,
                500
            );

            AddNullableString(
                cmd,
                "@sTechnicalSkillStarIDs",
                model.sTechnicalSkillStarIDs,
                100
            );

            AddNullableString(
                cmd,
                "@sNonTechnicalSkillIDs",
                model.sNonTechnicalSkillIDs,
                500
            );

            AddNullableString(
                cmd,
                "@sNonTechnicalSkillStarIDs",
                model.sNonTechnicalSkillStarIDs,
                100
            );

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // UPDATE DOCUMENTS
        // =========================================================

        // =========================================================
        // UPDATE DOCUMENTS / HOBBIES
        // =========================================================

        public void UpdateDocuments(CandidateProfileModel model)
        {
            using SqlConnection con = GetConnection();

            con.Open();

            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
        UPDATE tblCandidateProfile
        SET
            sResume = @sResume,
            sPhoto = @sPhoto,
            sSignature = @sSignature,
            sDivyang = @sDivyang,
            sHobbies = @sHobbies,
            ModDate = GETDATE()
        WHERE CandidateID = @CandidateID";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            cmd.Parameters.Add("@sResume", SqlDbType.NVarChar, 300)
                .Value = (object?)model.sResume ?? DBNull.Value;

            cmd.Parameters.Add("@sPhoto", SqlDbType.NVarChar, 300)
                .Value = (object?)model.sPhoto ?? DBNull.Value;

            cmd.Parameters.Add("@sSignature", SqlDbType.NVarChar, 300)
                .Value = (object?)model.sSignature ?? DBNull.Value;

            cmd.Parameters.Add("@sDivyang", SqlDbType.NVarChar, 20)
                .Value = (object?)model.sDivyang ?? DBNull.Value;

            cmd.Parameters.Add("@sHobbies", SqlDbType.NVarChar, 300)
                .Value = (object?)model.sHobbies ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // GET DIVISIONS
        // =========================================================

        public List<DropdownModel> GetDivisions()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblDivision ORDER BY nID"
            );
        }


        // =========================================================
        // GET STREAMS
        // =========================================================

        public List<DropdownModel> GetStreams()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblStream ORDER BY nID"
            );
        }


        // =========================================================
        // GET GRADUATION STATUS
        // =========================================================

        public List<DropdownModel> GetGraduationStatuses()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblGraduationstatus ORDER BY nID"
            );
        }


        // =========================================================
        // GET INTERNSHIP FELLOWSHIP TYPE
        // =========================================================

        public List<DropdownModel> GetInternshipFellowshipType()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblInternshipFellowshipType ORDER BY nID"
            );
        }


        // =========================================================
        // GET INTERNSHIP TITLES
        // =========================================================

        public List<DropdownModel> GetInternshipTitles()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblInternshipTitle ORDER BY nID"
            );
        }


        // =========================================================
        // GET INTERNSHIP DURATIONS
        // =========================================================

        public List<DropdownModel> GetInternshipDurations()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblInternshipsDoneDuration ORDER BY nID"
            );
        }


        // =========================================================
        // GET INTERNSHIP STATUS
        // =========================================================

        public List<DropdownModel> GetInternshipStatuses()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblInternshipStatus ORDER BY nID"
            );
        }


        // =========================================================
        // GET RELATIONSHIPS
        // =========================================================

        public List<DropdownModel> GetRelationships()
        {
            return GetDropdownData(
                "SELECT nID AS ID, sName AS Name FROM tblRelationship ORDER BY nID"
            );
        }


        // =========================================================
        // COMMON DROPDOWN METHOD
        // =========================================================

        private List<DropdownModel> GetDropdownData(
            string sql)
        {
            List<DropdownModel> list = new();

            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            con.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new DropdownModel
                {
                    ID =
                        Convert.ToInt32(reader["ID"]),

                    Name =
                        reader["Name"]?.ToString() ?? ""
                });
            }

            return list;
        }


        // =========================================================
        // PARAMETER HELPERS
        // =========================================================

        private void AddNullableInt(
            SqlCommand cmd,
            string parameterName,
            int? value)
        {
            cmd.Parameters.Add(
                parameterName,
                SqlDbType.Int
            ).Value =
                (object?)value ?? DBNull.Value;
        }


        private void AddNullableString(
            SqlCommand cmd,
            string parameterName,
            string? value,
            int size)
        {
            cmd.Parameters.Add(
                parameterName,
                SqlDbType.NVarChar,
                size
            ).Value =
                (object?)value ?? DBNull.Value;
        }


        // =========================================================
        // DATA READER HELPERS
        // =========================================================

        private int? GetNullableInt(
            SqlDataReader reader,
            string column)
        {
            if (reader[column] == DBNull.Value)
                return null;

            return Convert.ToInt32(
                reader[column]
            );
        }


        private string? GetNullableString(
            SqlDataReader reader,
            string column)
        {
            if (reader[column] == DBNull.Value)
                return null;

            return reader[column]?.ToString();
        }

        // =========================================================
        // UPDATE SOCIAL LINKS
        // =========================================================

        public void UpdateSocialLinks(CandidateProfileModel model)
        {
            using SqlConnection con = GetConnection();

            con.Open();

            // Make sure profile exists
            EnsureProfileExists(con, model.CandidateID);

            string sql = @"
 UPDATE tblCandidateProfile
 SET
     GitHub = @GitHub,
     Linkedin = @Linkedin,
     ModDate = GETDATE()
 WHERE CandidateID = @CandidateID";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@CandidateID", SqlDbType.Int)
                .Value = model.CandidateID;

            AddNullableString(
                cmd,
                "@GitHub",
                model.GitHub,
                500
            );

            AddNullableString(
                cmd,
                "@Linkedin",
                model.Linkedin,
                500
            );

            cmd.ExecuteNonQuery();
        }
    }

}
