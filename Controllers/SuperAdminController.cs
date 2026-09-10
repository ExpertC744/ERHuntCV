using ERHuntCV.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERHuntCV.Controllers
{
    public class SuperAdminController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly AccountRepository _repository;

        public SuperAdminController(IConfiguration configuration, AccountRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        // =====================================================
        // ADD IsValidSuperAdmin HERE
        // =====================================================
        private bool IsValidSuperAdmin(int id)
        {
            string? sessionId = HttpContext.Session.GetString("SAID");

            if (string.IsNullOrEmpty(sessionId))
                return false;

            if (!int.TryParse(sessionId, out int saId))
                return false;

            return id == saId;
        }

        private int GetSuperAdminId()
        {
            string? sessionId = HttpContext.Session.GetString("SAID");

            if (int.TryParse(sessionId, out int saId))
            {
                return saId;
            }

            return 0;
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        [Route("SuperAdmin/Dashboard/{id:int}")]
        public IActionResult Dashboard(int id)
        {
            if (id != 1)
            {
                return NotFound();
            }

            int traineeRegistrationCount = 0;
            int organizationRegistrationCount = 0;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT
                (SELECT COUNT(nID)
                 FROM tblCandidateReg) AS CandidateCount,

                (SELECT COUNT(nID)
                 FROM tblOrgRegistration) AS OrganizationCount;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        traineeRegistrationCount =
                            Convert.ToInt32(dr["CandidateCount"]);

                        organizationRegistrationCount =
                            Convert.ToInt32(dr["OrganizationCount"]);
                    }
                }
            }

            List<SATraineeListM> trainees =
                _repository.GetAllTrainees();

            List<OrganizationUserM> organizations =
                _repository.GetAllOrganizationList();

            ViewBag.TraineeRegistrationCount =
                traineeRegistrationCount;

            ViewBag.OrganizationRegistrationCount =
                organizationRegistrationCount;

            ViewBag.Trainees = trainees;

            ViewBag.Organizations = organizations;

            ViewBag.SuperAdminID = id;

            return View();
        }


        [HttpGet]
        [Route("SuperAdmin/SAFeedback/{id:int}")]
        public IActionResult SAFeedback(int id)
        {

            List<SACandidateFeedbackM> feedbackList = new List<SACandidateFeedbackM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5,
          nBit,
          nSABit,
          nSAID,
          RegDate,
          ModDate
      FROM tblSATRFeedback
      WHERE ISNULL(nBit, 1) = 1
      ORDER BY nID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            feedbackList.Add(new SACandidateFeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] != DBNull.Value
                                    ? dr["Que1"].ToString()
                                    : "",

                                Ans1 = dr["Ans1"] != DBNull.Value
                                    ? dr["Ans1"].ToString()
                                    : "",

                                Que2 = dr["Que2"] != DBNull.Value
                                    ? dr["Que2"].ToString()
                                    : "",

                                Ans2 = dr["Ans2"] != DBNull.Value
                                    ? dr["Ans2"].ToString()
                                    : "",

                                Que3 = dr["Que3"] != DBNull.Value
                                    ? dr["Que3"].ToString()
                                    : "",

                                Ans3 = dr["Ans3"] != DBNull.Value
                                    ? dr["Ans3"].ToString()
                                    : "",

                                Que4 = dr["Que4"] != DBNull.Value
                                    ? dr["Que4"].ToString()
                                    : "",

                                Ans4 = dr["Ans4"] != DBNull.Value
                                    ? dr["Ans4"].ToString()
                                    : "",

                                Que5 = dr["Que5"] != DBNull.Value
                                    ? dr["Que5"].ToString()
                                    : "",

                                Ans5 = dr["Ans5"] != DBNull.Value
                                    ? dr["Ans5"].ToString()
                                    : "",

                                nBit = dr["nBit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nBit"])
                                    : true,

                                nSABit = dr["nSABit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nSABit"])
                                    : true,

                                nSAID = dr["nSAID"] != DBNull.Value
                                    ? Convert.ToInt32(dr["nSAID"])
                                    : 0,

                                dRegDate = dr["RegDate"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["RegDate"])
                                    : null,

                                dModDate = dr["ModDate"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["ModDate"])
                                    : null
                            });
                        }
                    }
                }
            }
            ViewBag.SuperAdminID = id;
            return View(feedbackList);
        }


        [HttpGet]
        public IActionResult EditSAFeedback(int id)
        {

            SACandidateFeedbackM model = new SACandidateFeedbackM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          nSAID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5
      FROM tblSATRFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model.nID = Convert.ToInt32(dr["nID"]);

                            model.nSAID =
                                dr["nSAID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(dr["nSAID"]);

                            model.Que1 =
                                dr["Que1"] == DBNull.Value
                                ? null
                                : dr["Que1"].ToString();

                            model.Ans1 =
                                dr["Ans1"] == DBNull.Value
                                ? null
                                : dr["Ans1"].ToString();

                            model.Que2 =
                                dr["Que2"] == DBNull.Value
                                ? null
                                : dr["Que2"].ToString();

                            model.Ans2 =
                                dr["Ans2"] == DBNull.Value
                                ? null
                                : dr["Ans2"].ToString();

                            model.Que3 =
                                dr["Que3"] == DBNull.Value
                                ? null
                                : dr["Que3"].ToString();

                            model.Ans3 =
                                dr["Ans3"] == DBNull.Value
                                ? null
                                : dr["Ans3"].ToString();

                            model.Que4 =
                                dr["Que4"] == DBNull.Value
                                ? null
                                : dr["Que4"].ToString();

                            model.Ans4 =
                                dr["Ans4"] == DBNull.Value
                                ? null
                                : dr["Ans4"].ToString();

                            model.Que5 =
                                dr["Que5"] == DBNull.Value
                                ? null
                                : dr["Que5"].ToString();

                            model.Ans5 =
                                dr["Ans5"] == DBNull.Value
                                ? null
                                : dr["Ans5"].ToString();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSAFeedback(SACandidateFeedbackM model, int saId)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      UPDATE tblSATRFeedback
      SET
          Que1 = @Que1,
          Ans1 = @Ans1,
          Que2 = @Que2,
          Ans2 = @Ans2,
          Que3 = @Que3,
          Ans3 = @Ans3,
          Que4 = @Que4,
          Ans4 = @Ans4,
          Que5 = @Que5,
          Ans5 = @Ans5
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int)
                        .Value = model.nID;

                    cmd.Parameters.Add("@Que1", SqlDbType.NVarChar)
                        .Value = (object?)model.Que1 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans1", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans1 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que2", SqlDbType.NVarChar)
                        .Value = (object?)model.Que2 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans2", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans2 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que3", SqlDbType.NVarChar)
                        .Value = (object?)model.Que3 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans3", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans3 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que4", SqlDbType.NVarChar)
                        .Value = (object?)model.Que4 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans4", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans4 ?? DBNull.Value;

                    cmd.Parameters.Add("@Que5", SqlDbType.NVarChar)
                        .Value = (object?)model.Que5 ?? DBNull.Value;

                    cmd.Parameters.Add("@Ans5", SqlDbType.NVarChar)
                        .Value = (object?)model.Ans5 ?? DBNull.Value;


                    int rowsAffected = cmd.ExecuteNonQuery();


                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            TempData["SuccessMessage"] = "Feedback updated successfully.";

            return RedirectToAction("SAFeedback");
        }

        [HttpGet]
        public IActionResult DetailsSAFeedback(int id)
        {
            SACandidateFeedbackM model = new SACandidateFeedbackM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSATRFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model.nID = Convert.ToInt32(dr["nID"]);

                            model.Que1 = dr["Que1"]?.ToString();
                            model.Ans1 = dr["Ans1"]?.ToString();

                            model.Que2 = dr["Que2"]?.ToString();
                            model.Ans2 = dr["Ans2"]?.ToString();

                            model.Que3 = dr["Que3"]?.ToString();
                            model.Ans3 = dr["Ans3"]?.ToString();

                            model.Que4 = dr["Que4"]?.ToString();
                            model.Ans4 = dr["Ans4"]?.ToString();

                            model.Que5 = dr["Que5"]?.ToString();
                            model.Ans5 = dr["Ans5"]?.ToString();

                            model.nBit = dr["nBit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nBit"]);

                            model.nSABit = dr["nSABit"] != DBNull.Value &&
                                           Convert.ToBoolean(dr["nSABit"]);

                            model.nSAID = dr["nSAID"] != DBNull.Value
                                ? Convert.ToInt32(dr["nSAID"])
                                : 0;

                            model.dRegDate = dr["dRegDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dRegDate"])
                                : DateTime.MinValue;

                            model.dModDate = dr["dModDate"] != DBNull.Value
                                ? Convert.ToDateTime(dr["dModDate"])
                                : DateTime.MinValue;
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }


            return View(model);
        }

        [HttpGet]
        [Route("SuperAdmin/SAOrgFeedback/{id:int}")]
        public IActionResult SAOrgFeedback(int id)
        {

            List<SAOrgFeedbackM> feedbackList =
                new List<SAOrgFeedbackM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1,
          Ans1,
          Que2,
          Ans2,
          Que3,
          Ans3,
          Que4,
          Ans4,
          Que5,
          Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSAOrgFeedback
      WHERE ISNULL(nBit, 1) = 1
      ORDER BY nID DESC";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            feedbackList.Add(new SAOrgFeedbackM
                            {
                                nID =
                                    dr["nID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nID"])
                                        : 0,

                                Que1 =
                                    dr["Que1"] != DBNull.Value
                                        ? dr["Que1"].ToString()
                                        : "",

                                Ans1 =
                                    dr["Ans1"] != DBNull.Value
                                        ? dr["Ans1"].ToString()
                                        : "",

                                Que2 =
                                    dr["Que2"] != DBNull.Value
                                        ? dr["Que2"].ToString()
                                        : "",

                                Ans2 =
                                    dr["Ans2"] != DBNull.Value
                                        ? dr["Ans2"].ToString()
                                        : "",

                                Que3 =
                                    dr["Que3"] != DBNull.Value
                                        ? dr["Que3"].ToString()
                                        : "",

                                Ans3 =
                                    dr["Ans3"] != DBNull.Value
                                        ? dr["Ans3"].ToString()
                                        : "",

                                Que4 =
                                    dr["Que4"] != DBNull.Value
                                        ? dr["Que4"].ToString()
                                        : "",

                                Ans4 =
                                    dr["Ans4"] != DBNull.Value
                                        ? dr["Ans4"].ToString()
                                        : "",

                                Que5 =
                                    dr["Que5"] != DBNull.Value
                                        ? dr["Que5"].ToString()
                                        : "",

                                Ans5 =
                                    dr["Ans5"] != DBNull.Value
                                        ? dr["Ans5"].ToString()
                                        : "",

                                nBit =
                                    dr["nBit"] != DBNull.Value
                                        ? Convert.ToBoolean(dr["nBit"])
                                        : true,

                                nSABit =
                                    dr["nSABit"] != DBNull.Value
                                        ? Convert.ToBoolean(dr["nSABit"])
                                        : true,

                                nSAID =
                                    dr["nSAID"] != DBNull.Value
                                        ? Convert.ToInt32(dr["nSAID"])
                                        : 0,

                                dRegDate =
                                    dr["dRegDate"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["dRegDate"])
                                        : null,

                                dModDate =
                                    dr["dModDate"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["dModDate"])
                                        : null
                            });
                        }
                    }
                }
            }
            ViewBag.SuperAdminID = id;
            return View(feedbackList);
        }

        [HttpGet]
        public IActionResult EditSAOrgFeedback(int id, int saId)
        {
            SAOrgFeedbackM model = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1, Ans1,
          Que2, Ans2,
          Que3, Ans3,
          Que4, Ans4,
          Que5, Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSAOrgFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int).Value = id;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new SAOrgFeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] == DBNull.Value ? "" : dr["Que1"].ToString(),
                                Ans1 = dr["Ans1"] == DBNull.Value ? "" : dr["Ans1"].ToString(),

                                Que2 = dr["Que2"] == DBNull.Value ? "" : dr["Que2"].ToString(),
                                Ans2 = dr["Ans2"] == DBNull.Value ? "" : dr["Ans2"].ToString(),

                                Que3 = dr["Que3"] == DBNull.Value ? "" : dr["Que3"].ToString(),
                                Ans3 = dr["Ans3"] == DBNull.Value ? "" : dr["Ans3"].ToString(),

                                Que4 = dr["Que4"] == DBNull.Value ? "" : dr["Que4"].ToString(),
                                Ans4 = dr["Ans4"] == DBNull.Value ? "" : dr["Ans4"].ToString(),

                                Que5 = dr["Que5"] == DBNull.Value ? "" : dr["Que5"].ToString(),
                                Ans5 = dr["Ans5"] == DBNull.Value ? "" : dr["Ans5"].ToString(),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"]),

                                nSAID = dr["nSAID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nSAID"]),

                                dRegDate = dr["dRegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dRegDate"]),

                                dModDate = dr["dModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dModDate"])
                            };
                        }
                    }
                }
            }

            if (model == null)
                return NotFound();


            return View(model);
        }

        // =========================================================
        // SA ORGANIZATION FEEDBACK - UPDATE - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSAOrgFeedback(SAOrgFeedbackM model, int saId)
        {

            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
          UPDATE tblSAOrgFeedback
          SET
              Que1 = @Que1,
              Ans1 = @Ans1,

              Que2 = @Que2,
              Ans2 = @Ans2,

              Que3 = @Que3,
              Ans3 = @Ans3,

              Que4 = @Que4,
              Ans4 = @Ans4,

              Que5 = @Que5,
              Ans5 = @Ans5,

              dModDate = GETDATE()

          WHERE nID = @nID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@nID", SqlDbType.Int)
                            .Value = model.nID;

                        cmd.Parameters.Add("@Que1", SqlDbType.NVarChar)
                            .Value = (object?)model.Que1 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans1", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans1 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que2", SqlDbType.NVarChar)
                            .Value = (object?)model.Que2 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans2", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans2 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que3", SqlDbType.NVarChar)
                            .Value = (object?)model.Que3 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans3", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans3 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que4", SqlDbType.NVarChar)
                            .Value = (object?)model.Que4 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans4", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans4 ?? DBNull.Value;

                        cmd.Parameters.Add("@Que5", SqlDbType.NVarChar)
                            .Value = (object?)model.Que5 ?? DBNull.Value;

                        cmd.Parameters.Add("@Ans5", SqlDbType.NVarChar)
                            .Value = (object?)model.Ans5 ?? DBNull.Value;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            TempData["ErrorMessage"] =
                                "Feedback record not found.";

                            return View(model);
                        }
                    }
                }

                TempData["SuccessMessage"] =
                    "Feedback updated successfully.";

                return RedirectToAction("SAOrgFeedback", new { id = saId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: " + ex.Message;



                return View(model);
            }
        }


        [HttpGet]
        public IActionResult DetailsSAOrgFeedback(int id, int saId)
        {

            SAOrgFeedbackM model = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
      SELECT
          nID,
          Que1, Ans1,
          Que2, Ans2,
          Que3, Ans3,
          Que4, Ans4,
          Que5, Ans5,
          nBit,
          nSABit,
          nSAID,
          dRegDate,
          dModDate
      FROM tblSAOrgFeedback
      WHERE nID = @nID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@nID", SqlDbType.Int)
                        .Value = id;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new SAOrgFeedbackM
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] == DBNull.Value
                                    ? ""
                                    : dr["Que1"].ToString(),

                                Ans1 = dr["Ans1"] == DBNull.Value
                                    ? ""
                                    : dr["Ans1"].ToString(),

                                Que2 = dr["Que2"] == DBNull.Value
                                    ? ""
                                    : dr["Que2"].ToString(),

                                Ans2 = dr["Ans2"] == DBNull.Value
                                    ? ""
                                    : dr["Ans2"].ToString(),

                                Que3 = dr["Que3"] == DBNull.Value
                                    ? ""
                                    : dr["Que3"].ToString(),

                                Ans3 = dr["Ans3"] == DBNull.Value
                                    ? ""
                                    : dr["Ans3"].ToString(),

                                Que4 = dr["Que4"] == DBNull.Value
                                    ? ""
                                    : dr["Que4"].ToString(),

                                Ans4 = dr["Ans4"] == DBNull.Value
                                    ? ""
                                    : dr["Ans4"].ToString(),

                                Que5 = dr["Que5"] == DBNull.Value
                                    ? ""
                                    : dr["Que5"].ToString(),

                                Ans5 = dr["Ans5"] == DBNull.Value
                                    ? ""
                                    : dr["Ans5"].ToString(),

                                nBit = dr["nBit"] != DBNull.Value &&
                                       Convert.ToBoolean(dr["nBit"]),

                                nSABit = dr["nSABit"] != DBNull.Value &&
                                         Convert.ToBoolean(dr["nSABit"]),

                                nSAID = dr["nSAID"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(dr["nSAID"]),

                                dRegDate = dr["dRegDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dRegDate"]),

                                dModDate = dr["dModDate"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(dr["dModDate"])
                            };
                        }
                    }
                }
            }

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpGet]
        [Route("SuperAdmin/CandidateFeedback/{id:int}")]
        public IActionResult CandidateFeedback(int id)
        {

            List<SAFeedbackM> feedbackList = new List<SAFeedbackM>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
    WITH LatestFeedback AS
    (
        SELECT
            CF.nID AS FeedbackID,
            CF.nAdminID,
            CF.nSABit,
            ROW_NUMBER() OVER
            (
                PARTITION BY CF.nAdminID
                ORDER BY CF.nID DESC
            ) AS RowNum
        FROM tblCandidateFeedback CF
    )

    SELECT
        CR.nID,
        CR.sFName,
        CR.sLName,
        LF.FeedbackID,
        LF.nAdminID,
        ISNULL(LF.nSABit, 0) AS nSABit

    FROM tblCandidateReg CR

    INNER JOIN LatestFeedback LF
        ON CR.nID = LF.nAdminID

    WHERE LF.RowNum = 1

    ORDER BY LF.FeedbackID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SAFeedbackM model = new SAFeedbackM();

                            model.nID = Convert.ToInt32(dr["nID"]);
                            model.sFName = dr["sFName"]?.ToString();
                            model.sLName = dr["sLName"]?.ToString();

                            if (dr["nAdminID"] != DBNull.Value)
                            {
                                model.nAdminID = Convert.ToInt32(dr["nAdminID"]);
                            }

                            model.nSABit = Convert.ToInt32(dr["nSABit"]);

                            model.Status =
                                model.nSABit == 1 ? "Enabled" : "Disabled";

                            feedbackList.Add(model);
                        }
                    }
                }
            }

            return View(feedbackList);
        }

        //Enable / Disable Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleCandidateFeedback(int id, int saId)
        {

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            UPDATE tblCandidateFeedback
            SET
                nSABit = CASE
                            WHEN ISNULL(nSABit, 0) = 1
                                THEN 0
                            ELSE 1
                         END,
                ModDate = GETDATE()
            WHERE nAdminID = @id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int result = cmd.ExecuteNonQuery();

                    if (result == 0)
                    {
                        TempData["Error"] =
                            "Feedback record not found.";
                    }
                }
            }

            return RedirectToAction("CandidateFeedback", new { id = id });
        }


        [HttpGet]
        public IActionResult DetailsCandidateFeedback(int id, int saId)
        {

            SACandidateFeedbackDetailsM model =
                new SACandidateFeedbackDetailsM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT
                CR.nID,
                CR.sFName,
                CR.sLName,
                CR.sProfileImage,

                CF.nAdminID,
                CF.sQue1,
                CF.sQue2,
                CF.sQue3,
                CF.sQue4,
                CF.sQue5,
                CF.nSABit,
                CF.RegDate,
                CF.ModDate

            FROM tblCandidateReg CR

            INNER JOIN tblCandidateFeedback CF
                ON CR.nID = CF.nAdminID

            WHERE CR.nID = @nID
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Candidate details
                            model.nID =
                                Convert.ToInt32(dr["nID"]);

                            model.sFName =
                                dr["sFName"] == DBNull.Value
                                ? ""
                                : dr["sFName"].ToString();

                            model.sLName =
                                dr["sLName"] == DBNull.Value
                                ? ""
                                : dr["sLName"].ToString();

                            model.sProfileImage =
                                dr["sProfileImage"] == DBNull.Value
                                ? ""
                                : dr["sProfileImage"].ToString();


                            // Feedback details
                            model.nAdminID =
                                Convert.ToInt32(dr["nAdminID"]);

                            model.sQue1 =
                                Convert.ToInt32(dr["sQue1"]);

                            model.sQue2 =
                                Convert.ToInt32(dr["sQue2"]);

                            model.sQue3 =
                                Convert.ToInt32(dr["sQue3"]);

                            model.sQue4 =
                                Convert.ToInt32(dr["sQue4"]);

                            model.sQue5 =
                                dr["sQue5"] == DBNull.Value
                                ? ""
                                : dr["sQue5"].ToString();


                            // Super Admin status
                            model.nSABit =
                                dr["nSABit"] != DBNull.Value &&
                                Convert.ToBoolean(dr["nSABit"]);


                            // Dates
                            if (dr["RegDate"] != DBNull.Value)
                            {
                                model.RegDate =
                                    Convert.ToDateTime(dr["RegDate"]);
                            }

                            if (dr["ModDate"] != DBNull.Value)
                            {
                                model.ModDate =
                                    Convert.ToDateTime(dr["ModDate"]);
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            return View(model);
        }


        [HttpGet]
        [Route("SuperAdmin/OrganizationFeedback/{id:int}")]
        public IActionResult OrganizationFeedback(int id)
        {

            List<SAOrganizationFeedbackM> feedbackList =
                new List<SAOrganizationFeedbackM>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
WITH LatestFeedback AS
(
    SELECT
        F.nID AS FeedbackID,
        F.nSAID,
        F.nSABit,

        ROW_NUMBER() OVER
        (
            PARTITION BY F.nSAID
            ORDER BY F.nID DESC
        ) AS RowNum

    FROM tblSAOrgFeedback F
)

SELECT
    O.nID,
    O.sOrgName,
    LF.FeedbackID,
    ISNULL(LF.nSABit, 0) AS nSABit

FROM tblOrgRegistration O

INNER JOIN LatestFeedback LF
    ON O.nID = LF.nSAID

WHERE LF.RowNum = 1

ORDER BY LF.FeedbackID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SAOrganizationFeedbackM model =
                                new SAOrganizationFeedbackM();

                            model.nID =
                                Convert.ToInt32(dr["nID"]);

                            model.sOrgName =
                                dr["sOrgName"] == DBNull.Value
                                    ? ""
                                    : dr["sOrgName"].ToString();

                            model.nSABit =
                                Convert.ToInt32(dr["nSABit"]);

                            model.Status =
                                model.nSABit == 1
                                    ? "Enabled"
                                    : "Disabled";

                            feedbackList.Add(model);
                        }
                    }
                }
            }

            return View(feedbackList);
        }


        // Enable / Disable Organization Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleOrganizationFeedback(int id, int saId)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            UPDATE tblSAOrgFeedback
            SET
                nSABit = CASE
                            WHEN ISNULL(nSABit, 0) = 1 THEN 0
                            ELSE 1
                         END,
                dModDate = GETDATE()
            WHERE nID = @id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        TempData["Success"] =
                            "Organization feedback status updated successfully.";
                    }
                    else
                    {
                        TempData["Error"] =
                            "Organization feedback record not found.";
                    }
                }
            }

            return RedirectToAction("OrganizationFeedback", new { id = id });
        }

        [HttpGet]
        public IActionResult DetailsOrgFeedback(int id, int saId)
        {

            SAOrgFeedbackDetailsM model = new SAOrgFeedbackDetailsM();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
SELECT TOP 1

    /* ================= ORGANIZATION ================= */
    ORG.nID,
    ORG.sOrgName,

    /* ================= ORGANIZATION PROFILE ================= */
    OP.sCompanyLogo,

    /* ================= SA FEEDBACK QUESTIONS ================= */
    SAF.nID AS FeedbackID,
    SAF.nSAID,

    SAF.Que1,
    SAF.Que2,
    SAF.Que3,
    SAF.Que4,
    SAF.Que5,

    /* ================= ORGANIZATION ANSWERS ================= */
    OFB.sQue1 AS Ans1,
    OFB.sQue2 AS Ans2,
    OFB.sQue3 AS Ans3,
    OFB.sQue4 AS Ans4,
    OFB.sQue5 AS Ans5,

    /* ================= STATUS ================= */
    OFB.nSABit,

    /* ================= DATES ================= */
    OFB.RegDate,
    OFB.ModDate

FROM tblOrgRegistration ORG

/* Organization Profile */
LEFT JOIN tblOrgProfile OP
    ON OP.nOrgID = ORG.nID

/* SA defined feedback questions */
INNER JOIN tblSAOrgFeedback SAF
    ON SAF.nSAID = ORG.nID

/* Organization submitted answers */
INNER JOIN tblOrgFeedback OFB
    ON OFB.nAdminID = ORG.nID

WHERE ORG.nID = @OrgID

ORDER BY SAF.nID DESC, OFB.nID DESC;
";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@OrgID", SqlDbType.Int).Value = id;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            return NotFound();
                        }

                        // =========================================
                        // ORGANIZATION
                        // =========================================

                        model.nID = dr["nID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dr["nID"]);

                        model.sOrgName = dr["sOrgName"] == DBNull.Value
                            ? ""
                            : dr["sOrgName"].ToString();

                        // =========================================
                        // ORGANIZATION LOGO
                        // =========================================

                        model.sOrgLogo = dr["sCompanyLogo"] == DBNull.Value
                            ? ""
                            : dr["sCompanyLogo"].ToString();

                        // =========================================
                        // FEEDBACK ID
                        // =========================================

                        model.FeedbackID = dr["FeedbackID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dr["FeedbackID"]);

                        model.nSAID = dr["nSAID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(dr["nSAID"]);

                        // =========================================
                        // QUESTION 1
                        // Question comes from tblSAOrgFeedback
                        // Answer comes from tblOrgFeedback
                        // =========================================

                        model.sQue1 = dr["Que1"] == DBNull.Value
                            ? ""
                            : dr["Que1"].ToString();

                        model.sAns1 = dr["Ans1"] == DBNull.Value
                            ? ""
                            : dr["Ans1"].ToString();

                        // =========================================
                        // QUESTION 2
                        // =========================================

                        model.sQue2 = dr["Que2"] == DBNull.Value
                            ? ""
                            : dr["Que2"].ToString();

                        model.sAns2 = dr["Ans2"] == DBNull.Value
                            ? ""
                            : dr["Ans2"].ToString();

                        // =========================================
                        // QUESTION 3
                        // =========================================

                        model.sQue3 = dr["Que3"] == DBNull.Value
                            ? ""
                            : dr["Que3"].ToString();

                        model.sAns3 = dr["Ans3"] == DBNull.Value
                            ? ""
                            : dr["Ans3"].ToString();

                        // =========================================
                        // QUESTION 4
                        // =========================================

                        model.sQue4 = dr["Que4"] == DBNull.Value
                            ? ""
                            : dr["Que4"].ToString();

                        model.sAns4 = dr["Ans4"] == DBNull.Value
                            ? ""
                            : dr["Ans4"].ToString();

                        // =========================================
                        // QUESTION 5
                        // =========================================

                        model.sQue5 = dr["Que5"] == DBNull.Value
                            ? ""
                            : dr["Que5"].ToString();

                        model.sAns5 = dr["Ans5"] == DBNull.Value
                            ? ""
                            : dr["Ans5"].ToString();

                        // =========================================
                        // STATUS
                        // =========================================

                        model.nSABit =
                            dr["nSABit"] != DBNull.Value &&
                            Convert.ToBoolean(dr["nSABit"]);

                        // =========================================
                        // REG DATE
                        // =========================================

                        if (dr["RegDate"] != DBNull.Value)
                        {
                            model.RegDate =
                                Convert.ToDateTime(dr["RegDate"]);
                        }

                        // =========================================
                        // MOD DATE
                        // =========================================

                        if (dr["ModDate"] != DBNull.Value)
                        {
                            model.ModDate =
                                Convert.ToDateTime(dr["ModDate"]);
                        }
                    }
                }
            }

            return View(model);
        }

        // ==========================================
        // CANDIDATE LIST
        // ==========================================
        [HttpGet]
        [Route("SuperAdmin/SATrainees/{id:int}")]
        public IActionResult SATrainees(int id)
        {


            List<SATraineeListM> trainees = _repository.GetSATraineeList();

            return View(trainees);
        }

        // ==========================================
        // ORGANIZATION LIST
        // ==========================================
        [HttpGet]
        [Route("SuperAdmin/SAOrganizations/{id:int}")]
        public IActionResult SAOrganizations(int id)
        {
            List<OrganizationUserM> organization =
                _repository.GetAllOrganizationList();

            if (organization == null)
            {
                organization = new List<OrganizationUserM>();
            }

            ViewBag.SuperAdminID = id;

            return View(organization);
        }

    }
}
