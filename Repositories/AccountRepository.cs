using ERHuntCV.Models;
using ErJobPortal.Models;
using HuntCV_Portal.Models;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.Common;

namespace HuntCV_Portal.Repositories
{
    public class AccountRepository
    {
        private readonly IConfiguration _configuration;


        public AccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public int Register(CandidateRegisterM model)
        {
            int candidateID = 1;

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "DefaultConnection connection string not found.");
            }

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand("SP_AddCandidateReg", cn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@sFName",
                        model.sFName);

                    cmd.Parameters.AddWithValue(
                        "@sLName",
                        (object?)model.sLName ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@nGender",
                        (object?)model.nGender ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@DOB",
                        (object?)model.DOB ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sMobile",
                        model.sMobile);

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        model.sEmail);

                    cmd.Parameters.AddWithValue(
                        "@sOTP",
                        (object?)model.sOTP ?? DBNull.Value);

                    cmd.Parameters.AddWithValue(
             "@sProfileImage",
             string.IsNullOrWhiteSpace(model.sProfileImage)
                 ? (object)DBNull.Value
                 : model.sProfileImage);

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        model.sPassword);

                    cmd.Parameters.AddWithValue("@sCaptcha", (object?)model.sCaptcha ?? DBNull.Value);

                    cn.Open();

                    object? result =
                        cmd.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        candidateID =
                            Convert.ToInt32(result);
                    }
                }
            }

            return candidateID;
        }

        public (int CandidateID, DateTime? DOB, DateTime? RegDate)?
      GetCandidateRegistrationDetails(int candidateId)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                nID,
                DOB,
                RegDate
            FROM tblCandidateReg
            WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = candidateId;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int id = Convert.ToInt32(dr["nID"]);

                            DateTime? dob =
                                dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DOB"]);

                            DateTime? regDate =
                                dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]);

                            return (id, dob, regDate);
                        }
                    }
                }
            }

            return null;
        }

        // candidate login
        public CandidateRegisterM? LoginCandidate(CandidateLoginM model)
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "DefaultConnection is not configured.");
            }

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand("SP_CandidateLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        model.sEmail ?? "");

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        model.sPassword ?? "");

                    con.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new CandidateRegisterM
                            {
                                nID = Convert.ToInt32(
                                    dr["nID"]),

                                sFName =
                                    dr["sFName"]?.ToString(),

                                sLName =
                                    dr["sLName"]?.ToString(),

                                nGender =
                                    dr["nGender"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(
                                        dr["nGender"]),

                                DOB =
                                    dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(
                                        dr["DOB"]),

                                sMobile =
                                    dr["sMobile"]?.ToString(),

                                sEmail =
                                    dr["sEmail"]?.ToString(),

                                sProfileImage =
                                    dr["sProfileImage"]?.ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        // Organization Register
        public int RegisterOrganization(OrganizationRegisterM model)
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");
            }

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                    new SqlCommand("SP_RegisterOrganization", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Organization Name
                    cmd.Parameters.AddWithValue(
                        "@sOrgName",
                        (object?)model.sOrgName ?? DBNull.Value
                    );

                    // Organization Website
                    cmd.Parameters.AddWithValue(
                        "@sOrgUrl",
                        (object?)model.sOrgUrl ?? DBNull.Value
                    );

                    // Contact Person
                    cmd.Parameters.AddWithValue(
                        "@sName",
                        (object?)model.sName ?? DBNull.Value
                    );

                    // Designation
                    cmd.Parameters.AddWithValue(
                        "@sDesignation",
                        (object?)model.sDesignation ?? DBNull.Value
                    );

                    // Mobile Number
                    cmd.Parameters.AddWithValue(
                        "@sMobile",
                        (object?)model.sMobile ?? DBNull.Value
                    );

                    // Email Address
                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        (object?)model.sEmail ?? DBNull.Value
                    );

                    // Password
                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        (object?)model.sPassword ?? DBNull.Value
                    );

                    // OTP
                    cmd.Parameters.AddWithValue(
                        "@sOTP",
                        (object?)model.sOTP ?? DBNull.Value
                    );

                    cn.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public (int OrganizationID, DateTime? RegDate)?
    GetOrganizationRegistrationDetails(int orgId)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                nID,
                RegDate
            FROM tblOrgRegistration
            WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = orgId;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int organizationId =
                                Convert.ToInt32(dr["nID"]);

                            DateTime? regDate =
                                dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]);

                            return (organizationId, regDate);
                        }
                    }
                }
            }

            return null;
        }

        //org Login
        public OrganizationRegisterM? OrganizationLogin(OrganizationLoginM model)
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");
            }

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_OrganizationLogin",
                           cn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@sEmail",
                        (object?)model.sEmail ??
                        DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "@sPassword",
                        (object?)model.sPassword ??
                        DBNull.Value);

                    cn.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new OrganizationRegisterM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                sOrgName =
                                    dr["sOrgName"]?.ToString(),

                                sName =
                                    dr["sName"]?.ToString(),

                                sEmail =
                                    dr["sEmail"]?.ToString(),

                                sMobile =
                                    dr["sMobile"]?.ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        //SA login
        public SALoginM Login(string email, string password)
        {
            SALoginM? model = null;

            string? connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "DefaultConnection not found.");
            }

            using (SqlConnection con =
           new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"SELECT *
                  FROM tblSuperAdmin
                  WHERE nBit = 1
                  AND sEmail = @Email
                  AND sPassword = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new SALoginM
                            {
                                nID = Convert.ToInt32(dr["nID"]),
                                SAID = Convert.ToInt32(dr["SAID"]),
                                sFName = dr["sFName"].ToString(),
                                sLName = dr["sLName"].ToString(),
                                sEmail = dr["sEmail"].ToString(),
                                sMobile = dr["sMobile"].ToString(),
                                sRole = dr["sRole"].ToString()
                            };
                        }
                    }
                }
            }

            return model;
        }

        // =========================================================
        // GET ORGANIZATION PROFILE
        // =========================================================

        public OrgProfileM? GetOrganizationProfile(int orgId)
        {
            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection")!;

            OrgProfileM? model = null;

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_GetOrganizationProfile",
                           cn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value =
                        orgId;

                    cn.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new OrgProfileM
                            {
                                nID =
                                    dr["nID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nID"])
                                        : 0,

                                nOrgID =
                                    dr["nOrgID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nOrgID"])
                                        : orgId,

                                sOrganizationName =
                                    dr["sName"] != DBNull.Value
                                        ? Convert.ToString(dr["sName"]) ?? ""
                                        : "",

                                sOrganizationEmail =
                                    dr["sEmail"] != DBNull.Value
                                        ? Convert.ToString(dr["sEmail"]) ?? ""
                                        : "",

                                sMobile =
                                    dr["sMobile"] != DBNull.Value
                                        ? Convert.ToString(dr["sMobile"]) ?? ""
                                        : "",

                                sDesignation =
                                    dr["sDesignation"] != DBNull.Value
                                        ? Convert.ToString(dr["sDesignation"]) ?? ""
                                        : "",

                                dDateOfBirth =
                                    dr["dDateOfBirth"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["dDateOfBirth"])
                                        : null,

                                sCompanyLogo =
                                    dr["sCompanyLogo"] != DBNull.Value
                                        ? Convert.ToString(dr["sCompanyLogo"]) ?? ""
                                        : "",

                                sCompanyAddress =
                                    dr["sCompanyAddress"] != DBNull.Value
                                        ? Convert.ToString(dr["sCompanyAddress"]) ?? ""
                                        : "",

                                nEstablishmentYear =
                                    dr["nEstablishmentYear"] != DBNull.Value
                                        ? Convert.ToInt32(
                                            dr["nEstablishmentYear"])
                                        : 0,

                                sGSTNo =
                                    dr["sGSTNo"] != DBNull.Value
                                        ? Convert.ToString(dr["sGSTNo"]) ?? ""
                                        : "",

                                sCINNo =
                                    dr["sCINNo"] != DBNull.Value
                                        ? Convert.ToString(dr["sCINNo"]) ?? ""
                                        : "",

                                nEmployeeStrength =
                                    dr["nEmployeeStrength"] != DBNull.Value
                                        ? Convert.ToString(
                                            dr["nEmployeeStrength"]) ?? ""
                                        : "",

                                dCreatedDate =
                                    dr["dCreatedDate"] != DBNull.Value
                                        ? Convert.ToDateTime(
                                            dr["dCreatedDate"])
                                        : DateTime.MinValue,

                                dModifiedDate =
                                    dr["dModifiedDate"] != DBNull.Value
                                        ? Convert.ToDateTime(
                                            dr["dModifiedDate"])
                                        : null,

                                nBit =
                                    dr["nBit"] != DBNull.Value &&
                                    Convert.ToBoolean(dr["nBit"]),

                                nSABit =
                                    dr["nSABit"] != DBNull.Value
                                        ? Convert.ToBoolean(
                                            dr["nSABit"])
                                        : null
                            };
                        }
                    }
                }
            }

            return model;
        }


        // =========================================================
        // SAVE ORGANIZATION PROFILE
        // =========================================================

        public bool SaveOrganizationProfile(
            OrgProfileM model,
            string logoPath)
        {
            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection")!;

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           "SP_AddOrganizationProfile",
                           cn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;


                    cmd.Parameters.Add(
                        "@nOrgID",
                        SqlDbType.Int).Value =
                        model.nOrgID;


                    cmd.Parameters.Add(
                        "@dDateOfBirth",
                        SqlDbType.DateTime).Value =
                        model.dDateOfBirth.HasValue
                            ? model.dDateOfBirth.Value
                            : DBNull.Value;


                    cmd.Parameters.Add(
                        "@sCompanyLogo",
                        SqlDbType.NVarChar,
                        500).Value =
                        string.IsNullOrWhiteSpace(logoPath)
                            ? DBNull.Value
                            : logoPath;


                    cmd.Parameters.Add(
                        "@sCompanyAddress",
                        SqlDbType.NVarChar,
                        -1).Value =
                        string.IsNullOrWhiteSpace(
                            model.sCompanyAddress)
                            ? DBNull.Value
                            : model.sCompanyAddress;


                    cmd.Parameters.Add(
                        "@nEstablishmentYear",
                        SqlDbType.Int).Value =
                        model.nEstablishmentYear;


                    cmd.Parameters.Add(
                        "@sGSTNo",
                        SqlDbType.NVarChar,
                        50).Value =
                        string.IsNullOrWhiteSpace(
                            model.sGSTNo)
                            ? DBNull.Value
                            : model.sGSTNo;


                    cmd.Parameters.Add(
                        "@sCINNo",
                        SqlDbType.NVarChar,
                        50).Value =
                        string.IsNullOrWhiteSpace(
                            model.sCINNo)
                            ? DBNull.Value
                            : model.sCINNo;


                    cmd.Parameters.Add(
                        "@nEmployeeStrength",
                        SqlDbType.NVarChar,
                        100).Value =
                        string.IsNullOrWhiteSpace(
                            model.nEmployeeStrength)
                            ? DBNull.Value
                            : model.nEmployeeStrength;


                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }


        public DataTable GetAllCandidate()
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            SELECT *
            FROM tblCandidateReg
            ORDER BY RegDate DESC";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        con.Open();

                        da.Fill(dt);

                        return dt;
                    }
                }
            }
        }


        public int UpdateCandidateStatus(
            int nID,
            bool nSABit)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE tblCandidateReg
            SET nSABit = @nSABit
            WHERE nID = @nID";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = nID;

                    cmd.Parameters.Add("@nSABit", SqlDbType.Bit).Value =
                        nSABit;

                    con.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }


        public DataTable GetAllOrganization()
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            SELECT *
            FROM tblOrgRegistration
            ORDER BY RegDate DESC";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        con.Open();

                        da.Fill(dt);

                        return dt;
                    }
                }
            }
        }


        public int UpdateOrganizationStatus(
            int nID,
            bool nSABit)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection connection string not found.");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE tblOrgRegistration
            SET nSABit = @nSABit,
                ModDate = GETDATE()
            WHERE nID = @nID";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = nID;

                    cmd.Parameters.Add("@nSABit", SqlDbType.Bit).Value =
                        nSABit;

                    con.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }


        #region "Super Admin Trainee List"

        public List<SATraineeListM> GetAllTrainees()
        {
            return GetSATraineeList();
        }

        public List<SATraineeListM> GetSATraineeList()
        {
            List<SATraineeListM> list = new List<SATraineeListM>();

            string connectionString =
       _configuration.GetConnectionString("DefaultConnection")
       ?? throw new InvalidOperationException(
           "DefaultConnection connection string not found.");

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                nID,
                sFName,
                sLName,
                sMobile,
                sEmail,
                DOB,
                nGender,
                sProfileImage,
                RegDate,
                ModDate,
                nBit,
                nSABit
            FROM tblCandidateReg
            WHERE nBit = 1
            ORDER BY nID DESC";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SATraineeListM model = new SATraineeListM
                            {
                                nID = dr["nID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nID"]),

                                sFName = dr["sFName"] == DBNull.Value
                                    ? ""
                                    : dr["sFName"].ToString()!,

                                sLName = dr["sLName"] == DBNull.Value
                                    ? ""
                                    : dr["sLName"].ToString()!,

                                sMobile = dr["sMobile"] == DBNull.Value
                                    ? ""
                                    : dr["sMobile"].ToString()!,

                                sEmail = dr["sEmail"] == DBNull.Value
                                    ? ""
                                    : dr["sEmail"].ToString()!,

                                DOB = dr["DOB"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["DOB"]),

                                nGender = dr["nGender"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nGender"]),

                                sProfileImage = dr["sProfileImage"] == DBNull.Value
                                    ? ""
                                    : dr["sProfileImage"].ToString()!,

                                RegDate = dr["RegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["RegDate"]),

                                ModDate = dr["ModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["ModDate"]),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"])
                            };

                            list.Add(model);
                        }
                    }
                }
            }

            return list;
        }

        #endregion

        #region Super Admin Organization List

        public List<OrganizationUserM> GetAllOrganizationList()
        {
            List<OrganizationUserM> list =
                new List<OrganizationUserM>();

            string connectionString =
     _configuration.GetConnectionString("DefaultConnection")
     ?? throw new InvalidOperationException(
         "DefaultConnection connection string not found.");

            using (SqlConnection cn =
                   new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                nID,
                sOrgName,
                sOrgUrl,
                sName,
                sDesignation,
                sMobile,
                sEmail,
                sPassword,
                RegDate,
                ModDate,
                nBit,
                nSABit
            FROM tblOrgRegistration
            WHERE nBit = 1
            ORDER BY nID DESC";

                using (SqlCommand cmd =
                       new SqlCommand(query, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrganizationUserM model =
                                new OrganizationUserM
                                {
                                    nID = dr["nID"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(dr["nID"]),

                                    sOrgName = dr["sOrgName"] == DBNull.Value
                                        ? ""
                                        : dr["sOrgName"].ToString(),

                                    sOrgUrl = dr["sOrgUrl"] == DBNull.Value
                                        ? ""
                                        : dr["sOrgUrl"].ToString(),

                                    sName = dr["sName"] == DBNull.Value
                                        ? ""
                                        : dr["sName"].ToString(),

                                    sDesignation = dr["sDesignation"] == DBNull.Value
                                        ? ""
                                        : dr["sDesignation"].ToString(),

                                    sMobile = dr["sMobile"] == DBNull.Value
                                        ? ""
                                        : dr["sMobile"].ToString(),

                                    sEmail = dr["sEmail"] == DBNull.Value
                                        ? ""
                                        : dr["sEmail"].ToString(),


                                    sPassword = dr["sPassword"] == DBNull.Value
                                        ? ""
                                        : dr["sPassword"].ToString(),

                                    RegDate = dr["RegDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(dr["RegDate"]),

                                    ModDate = dr["ModDate"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(dr["ModDate"]),

                                    nBit = dr["nBit"] != DBNull.Value &&
                                           Convert.ToBoolean(dr["nBit"]),

                                    nSABit = dr["nSABit"] != DBNull.Value &&
                                             Convert.ToBoolean(dr["nSABit"])
                                };

                            list.Add(model);
                        }
                    }
                }
            }

            return list;
        }

        #endregion

    }
}