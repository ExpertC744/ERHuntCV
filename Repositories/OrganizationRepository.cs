using Microsoft.Data.SqlClient;
using System.Data;
using ERHuntCV.Models;

namespace ERHuntCV.Repositories
{
    public class OrganizationRepository
    {
        private readonly IConfiguration _configuration;

        public OrganizationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "Database connection string not found.");
            }

            return new SqlConnection(connectionString);
        }


        // =========================================================
        // CREATE ORGANIZATION POST
        // =========================================================

        public int CreatePost(OrgPostM model, int orgID)
        {
            using SqlConnection con = GetConnection();

            using SqlCommand cmd = new SqlCommand(
                "SP_CreateOrgPost",
                con);

            cmd.CommandType = CommandType.StoredProcedure;


            // =====================================================
            // ROLE DETAILS
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@nPositionID",
                model.nPositionID);

            cmd.Parameters.AddWithValue(
                "@nRequiredTrainees",
                model.nRequiredTrainees);

            cmd.Parameters.AddWithValue(
                "@nGenderID",
                model.nGenderID);

            cmd.Parameters.AddWithValue(
                "@nMinimumQualificationID",
                model.nMinimumQualificationID);


            // =====================================================
            // LOCATION
            // =====================================================

            cmd.Parameters.AddWithValue(
     "@sCountryCode",
     string.IsNullOrWhiteSpace(model.CountryID)
         ? DBNull.Value
         : model.CountryID);

            cmd.Parameters.AddWithValue(
                "@sStateCode",
                string.IsNullOrWhiteSpace(model.StateID)
                    ? DBNull.Value
                    : model.StateID);

            cmd.Parameters.AddWithValue(
                "@nCityID",
                string.IsNullOrWhiteSpace(model.CityID)
                    ? DBNull.Value
                    : model.CityID);

            // =====================================================
            // WORK TERMS
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@nInternshipTypeID",
                model.nInternshipTypeID);

            cmd.Parameters.AddWithValue(
                "@sWorkingShift",
                (object?)model.sWorkingShift ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@nInternshipFellowshipTypeID",
                model.nInternshipFellowshipTypeID);

            cmd.Parameters.AddWithValue(
                "@sTotalCharges",
                model.sTotalCharges.HasValue
                    ? model.sTotalCharges.Value
                    : DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@sCurrency",
                (object?)model.sCurrency ?? DBNull.Value);


            // =====================================================
            // DURATION
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@nTrainingInvolvedID",
                model.nTrainingInvolvedID);

            cmd.Parameters.AddWithValue(
                "@nInternshipDurationID",
                model.nInternshipDurationID);

            cmd.Parameters.AddWithValue(
                "@dStartDate",
                model.dStartDate);

            cmd.Parameters.AddWithValue(
                "@dCompletionDate",
                model.dCompletionDate);


            // =====================================================
            // MODE
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@nInternshipModeID",
                model.nInternshipModeID);

            cmd.Parameters.AddWithValue(
                "@sDivyang",
                (object?)model.sDivyang ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@sLanguageKnown",
                (object?)model.sLanguageKnown ?? DBNull.Value);


            // =====================================================
            // WORKING DAYS / FACILITIES
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@sWorkingDays",
                string.IsNullOrWhiteSpace(model.sWorkingDays)
                    ? DBNull.Value
                    : model.sWorkingDays);

            cmd.Parameters.AddWithValue(
                "@sFacilities",
                string.IsNullOrWhiteSpace(model.sFacilities)
                    ? DBNull.Value
                    : model.sFacilities);


            // =====================================================
            // SKILLS
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@sMedicalSkills",
                string.IsNullOrWhiteSpace(model.sMedicalSkills)
                    ? DBNull.Value
                    : model.sMedicalSkills);

            cmd.Parameters.AddWithValue(
                "@sTechnicalSkills",
                string.IsNullOrWhiteSpace(model.sTechnicalSkills)
                    ? DBNull.Value
                    : model.sTechnicalSkills);

            cmd.Parameters.AddWithValue(
                "@sNonTechnicalSkills",
                string.IsNullOrWhiteSpace(model.sNonTechnicalSkills)
                    ? DBNull.Value
                    : model.sNonTechnicalSkills);


            // =====================================================
            // ORGANIZATION ID
            // =====================================================

            cmd.Parameters.AddWithValue(
                "@nOrgID",
                orgID);


            // =====================================================
            // EXECUTE
            // =====================================================

            con.Open();

            object? result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(result);
        }

        // =========================================================
        // POSITION
        // =========================================================

        public List<MasterM> GetPositions()
        {
            return GetMasterData("SELECT nID, sName FROM tblPositions");
        }


        // =========================================================
        // GENDER
        // =========================================================

        public List<MasterM> GetGenders()
        {
            return GetMasterData("SELECT nID, sName FROM tblGender");
        }


        // =========================================================
        // MINIMUM QUALIFICATION
        // =========================================================

        public List<MasterM> GetMinimumQualifications()
        {
            return GetMasterData(
                "SELECT nID, sName FROM tblMinimumQualification");
        }


        // =========================================================
        // INTERNSHIP TYPE
        // =========================================================

        public List<MasterM> GetInternshipTypes()
        {
            return GetMasterData(
                "SELECT nID, sName FROM tblInternshipType");
        }


        // =========================================================
        // INTERNSHIP / FELLOWSHIP TYPE
        // =========================================================

        public List<MasterM> GetInternshipFellowshipTypes()
        {
            return GetMasterData(
                "SELECT nID, sName FROM tblInternshipFellowshipType");
        }


        // =========================================================
        // TRAINING INVOLVED
        // =========================================================

        public List<MasterM> GetTrainingInvolved()
        {
            return GetMasterData(
                "SELECT nID, sName FROM tblTrainingInvolved");
        }


        // =========================================================
        // INTERNSHIP DURATION
        // =========================================================

        public List<MasterM> GetInternshipDurations()
        {
            return GetMasterData(
                "SELECT nID, sName FROM tblInternshipDuration");
        }


        // =========================================================
        // INTERNSHIP MODE
        // =========================================================

        public List<MasterM> GetInternshipModes()
        {
            return GetMasterData(
                "SELECT nID, sName FROM tblInternshipMode");
        }


        // =========================================================
        // COMMON MASTER METHOD
        // =========================================================

        private List<MasterM> GetMasterData(string query)
        {
            List<MasterM> list = new List<MasterM>();

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new MasterM
                            {
                                nID = dr["nID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nID"]),

                                sName = dr["sName"] == DBNull.Value
                                    ? string.Empty
                                    : dr["sName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }

            return list;
        }


        // =========================================================
        // TECHNICAL SKILLS
        // =========================================================

        public List<OrgPostM> GetTechnicalSkills()
        {
            List<OrgPostM> list = new List<OrgPostM>();

            using (SqlConnection con = GetConnection())
            {
                string query = @"
                    SELECT
                        nTechnicalSkillID,
                        sTechnicalSkillName
                    FROM tblTechnicalSkills";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new OrgPostM
                            {
                                nTechnicalSkillID =
                                    Convert.ToInt32(dr["nTechnicalSkillID"]),

                                sTechnicalSkillName =
                                    dr["sTechnicalSkillName"]?.ToString()
                                    ?? string.Empty
                            });
                        }
                    }
                }
            }

            return list;
        }


        // =========================================================
        // MEDICAL SKILLS
        // =========================================================

        public List<OrgPostM> GetMedicalSkills()
        {
            List<OrgPostM> list = new List<OrgPostM>();

            using (SqlConnection con = GetConnection())
            {
                string query = @"
                    SELECT
                        nMedicalSkillID,
                        sMedicalSkillName
                    FROM tblMedicalSkills";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new OrgPostM
                            {
                                nMedicalSkillID =
                                    Convert.ToInt32(dr["nMedicalSkillID"]),

                                sMedicalSkillName =
                                    dr["sMedicalSkillName"]?.ToString()
                                    ?? string.Empty
                            });
                        }
                    }
                }
            }

            return list;
        }


        // =========================================================
        // NON TECHNICAL SKILLS
        // =========================================================

        public List<OrgPostM> GetNonTechnicalSkills()
        {
            List<OrgPostM> list =
                new List<OrgPostM>();

            using (SqlConnection con = GetConnection())
            {
                string query = @"
                    SELECT
                        nNonTechnicalSkillID,
                        sNonTechnicalSkillName
                    FROM tblNonTechnicalSkills";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new OrgPostM
                            {
                                nNonTechnicalSkillID =
                                    Convert.ToInt32(
                                        dr["nNonTechnicalSkillID"]),

                                sNonTechnicalSkillName =
                                    dr["sNonTechnicalSkillName"]?.ToString()
                                    ?? string.Empty
                            });
                        }
                    }
                }
            }

            return list;
        }

        // =========================================================
        // GET ORGANIZATION POST BY ID
        // =========================================================

        public OrgPostM GetOrganizationPostById(
            int postId,
            int orgId)
        {
            OrgPostM model = new OrgPostM();

            using SqlConnection con = GetConnection();

            using SqlCommand cmd = new SqlCommand(
                @"SELECT
                    nID,
                    nOrgID,
                    sName,
                    nPositionID,
                    nRequiredTrainees,
                    nGenderID,
                    nMinimumQualificationID,

                    CountryID,
                    StateID,
                    CityID,

                    sWorkingHours,
                    nInternshipTypeID,
                    sWorkingShift,
                    nInternshipFellowshipTypeID,
                    sTotalCharges,
                    sCurrency,
                    nTrainingInvolvedID,
                    nInternshipDurationID,
                    dStartDate,
                    dCompletionDate,
                    nInternshipModeID,
                    sDivyang,
                    sLanguageKnown,
                    sWorkingDays,
                    sFacilities,
                    sMedicalSkills,
                    sTechnicalSkills,
                    sNonTechnicalSkills,

                    dRegisterDate,
                    dModDate,
                    nBit,
                    nSABit

                  FROM tblPost
                  WHERE nID = @nID
                  AND nOrgID = @nOrgID",
                con);

            cmd.Parameters.Add(
                "@nID",
                SqlDbType.Int).Value = postId;

            cmd.Parameters.Add(
                "@nOrgID",
                SqlDbType.Int).Value = orgId;

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                model.nID =
                    reader["nID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["nID"]);

                model.nOrgID =
                    reader["nOrgID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["nOrgID"]);

                model.sName =
                    reader["sName"]?.ToString();


                // =====================================================
                // LOCATION IDS
                // =====================================================

                model.CountryID =
                    reader["CountryID"] == DBNull.Value
                        ? null
                        : reader["CountryID"].ToString();

                model.StateID =
                    reader["StateID"] == DBNull.Value
                        ? null
                        : reader["StateID"].ToString();

                model.CityID =
                    reader["CityID"] == DBNull.Value
                        ? null
                        : reader["CityID"].ToString();


                // =====================================================
                // OTHER FIELDS
                // =====================================================

                model.nPositionID =
                    reader["nPositionID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["nPositionID"]);

                model.nRequiredTrainees =
                    reader["nRequiredTrainees"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["nRequiredTrainees"]);

                model.nGenderID =
                    reader["nGenderID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["nGenderID"]);

                model.nMinimumQualificationID =
                    reader["nMinimumQualificationID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            reader["nMinimumQualificationID"]);

                model.sWorkingHours =
                    reader["sWorkingHours"]?.ToString();

                model.nInternshipTypeID =
                    reader["nInternshipTypeID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            reader["nInternshipTypeID"]);

                model.sWorkingShift =
                    reader["sWorkingShift"]?.ToString();

                model.nInternshipFellowshipTypeID =
                    reader["nInternshipFellowshipTypeID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            reader["nInternshipFellowshipTypeID"]);

                model.sTotalCharges =
     reader["sTotalCharges"] == DBNull.Value
         ? null
         : Convert.ToDecimal(reader["sTotalCharges"]);

                model.sCurrency =
                    reader["sCurrency"]?.ToString();

                model.nTrainingInvolvedID =
                    reader["nTrainingInvolvedID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            reader["nTrainingInvolvedID"]);

                model.nInternshipDurationID =
                    reader["nInternshipDurationID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            reader["nInternshipDurationID"]);

                if (reader["dStartDate"] != DBNull.Value)
                {
                    model.dStartDate =
                        Convert.ToDateTime(
                            reader["dStartDate"]);
                }

                if (reader["dCompletionDate"] != DBNull.Value)
                {
                    model.dCompletionDate =
                        Convert.ToDateTime(
                            reader["dCompletionDate"]);
                }

                model.nInternshipModeID =
                    reader["nInternshipModeID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            reader["nInternshipModeID"]);

                model.sDivyang =
                    reader["sDivyang"]?.ToString();

                model.sLanguageKnown =
                    reader["sLanguageKnown"]?.ToString();

                model.sWorkingDays =
                    reader["sWorkingDays"]?.ToString();

                model.sFacilities =
                    reader["sFacilities"]?.ToString();

                model.sMedicalSkills =
                    reader["sMedicalSkills"]?.ToString();

                model.sTechnicalSkills =
                    reader["sTechnicalSkills"]?.ToString();

                model.sNonTechnicalSkills =
                    reader["sNonTechnicalSkills"]?.ToString();
            }

            return model;
        }


        // =========================================================
        // UPDATE LOCATION ONLY
        // =========================================================

        public void UpdateOrganizationLocation(
            int postId,
            int orgId,
            string? countryId,
            string? stateId,
            string? cityId)
        {
            using SqlConnection con = GetConnection();

            using SqlCommand cmd = new SqlCommand(
                @"UPDATE tblPost
                  SET
                      CountryID = @CountryID,
                      StateID = @StateID,
                      CityID = @CityID,
                      dModDate = GETDATE()
                  WHERE nID = @nID
                  AND nOrgID = @nOrgID",
                con);

            cmd.Parameters.Add(
                "@nID",
                SqlDbType.Int).Value = postId;

            cmd.Parameters.Add(
                "@nOrgID",
                SqlDbType.Int).Value = orgId;

            cmd.Parameters.Add(
                "@CountryID",
                SqlDbType.NVarChar, 50).Value =
                    (object?)countryId ?? DBNull.Value;

            cmd.Parameters.Add(
                "@StateID",
                SqlDbType.NVarChar, 50).Value =
                    (object?)stateId ?? DBNull.Value;

            cmd.Parameters.Add(
                "@CityID",
                SqlDbType.NVarChar, 50).Value =
                    (object?)cityId ?? DBNull.Value;

            con.Open();

            cmd.ExecuteNonQuery();
        }

        public int CreateOrganizationPost(OrgPostM model)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                    new SqlCommand("SP_CreateOrgPost", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@nPositionID", SqlDbType.Int)
                        .Value = model.nPositionID;

                    cmd.Parameters.Add("@nRequiredTrainees", SqlDbType.Int)
                        .Value = model.nRequiredTrainees;

                    cmd.Parameters.Add("@nGenderID", SqlDbType.Int)
                        .Value = model.nGenderID;

                    cmd.Parameters.Add("@nMinimumQualificationID", SqlDbType.Int)
                        .Value = model.nMinimumQualificationID;


                    // LOCATION
                    cmd.Parameters.Add("@sCountryCode", SqlDbType.NVarChar, 10)
                        .Value = model.CountryID ?? "";

                    cmd.Parameters.Add("@sStateCode", SqlDbType.NVarChar, 20)
                        .Value = model.StateID ?? "";

                    cmd.Parameters.Add("@nCityID", SqlDbType.Int)
                        .Value = model.CityID;


                    cmd.Parameters.Add("@sWorkingHours", SqlDbType.NVarChar, 50)
                        .Value = model.sWorkingHours ?? "";


                    // WORK TERMS
                    cmd.Parameters.Add("@nInternshipTypeID", SqlDbType.Int)
                        .Value = model.nInternshipTypeID;

                    cmd.Parameters.Add("@sWorkingShift", SqlDbType.VarChar, 50)
                        .Value = model.sWorkingShift ?? "";

                    cmd.Parameters.Add("@nInternshipFellowshipTypeID", SqlDbType.Int)
                        .Value = model.nInternshipFellowshipTypeID;


                    // TOTAL CHARGES
                    SqlParameter charges =
                        cmd.Parameters.Add("@sTotalCharges", SqlDbType.Decimal);

                    charges.Precision = 18;
                    charges.Scale = 2;

                    charges.Value =
                        model.sTotalCharges.HasValue
                            ? model.sTotalCharges.Value
                            : DBNull.Value;


                    cmd.Parameters.Add("@sCurrency", SqlDbType.NVarChar, 50)
                        .Value = model.sCurrency ?? "";


                    // DURATION
                    cmd.Parameters.Add("@nTrainingInvolvedID", SqlDbType.Int)
                        .Value = model.nTrainingInvolvedID;

                    cmd.Parameters.Add("@nInternshipDurationID", SqlDbType.Int)
                        .Value = model.nInternshipDurationID;

                    cmd.Parameters.Add("@dStartDate", SqlDbType.DateTime)
                        .Value = model.dStartDate;

                    cmd.Parameters.Add("@dCompletionDate", SqlDbType.DateTime)
                        .Value = model.dCompletionDate;


                    // MODE
                    cmd.Parameters.Add("@nInternshipModeID", SqlDbType.Int)
                        .Value = model.nInternshipModeID;

                    cmd.Parameters.Add("@sDivyang", SqlDbType.NVarChar, 10)
                        .Value = model.sDivyang ?? "";

                    cmd.Parameters.Add("@sLanguageKnown", SqlDbType.NVarChar, 200)
                        .Value = model.sLanguageKnown ?? "";


                    // WORKING DAYS
                    cmd.Parameters.Add("@sWorkingDays", SqlDbType.NVarChar, 500)
                        .Value = model.sWorkingDays ?? "";


                    // FACILITIES
                    cmd.Parameters.Add("@sFacilities", SqlDbType.NVarChar, 500)
                        .Value = model.sFacilities ?? "";


                    // SKILLS
                    cmd.Parameters.Add("@sMedicalSkills", SqlDbType.NVarChar, 1000)
                        .Value = model.sMedicalSkills ?? "";

                    cmd.Parameters.Add("@sTechnicalSkills", SqlDbType.NVarChar, 1000)
                        .Value = model.sTechnicalSkills ?? "";

                    cmd.Parameters.Add("@sNonTechnicalSkills", SqlDbType.NVarChar, 1000)
                        .Value = model.sNonTechnicalSkills ?? "";


                    // ORGANIZATION
                    cmd.Parameters.Add("@nOrgID", SqlDbType.Int)
                        .Value = model.nOrgID;


                    con.Open();

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        return 0;
                    }

                    return Convert.ToInt32(result);
                }
            }
        }
    }
}