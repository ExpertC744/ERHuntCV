using ERHuntCV.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HuntCV_Portal.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AccountRepository _accountRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly AccountRepository _repository;


        public OrganizationController(
            IConfiguration configuration,
            AccountRepository accountRepository,
            IWebHostEnvironment environment, AccountRepository repository)
        {
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));

            _accountRepository = accountRepository
                ?? throw new ArgumentNullException(nameof(accountRepository));

            _environment = environment
                ?? throw new ArgumentNullException(nameof(environment));

            _repository = repository;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateFeedback()
        {
            // =====================================================
            // GET CANDIDATE ID
            // =====================================================

            int? OrgID =
                HttpContext.Session.GetInt32("OrgID");

            if (OrgID == null || OrgID <= 0)
            {
                return RedirectToAction("CreateFeedback", "Organization");
            }

            CandidateFeedbackViewModel feedback = null;

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
                Que2,
                Que3,
                Que4,
                Que5,
                nBit,
                nSABit
            FROM tblSAOrgFeedback
            WHERE nID = 1
              AND ISNULL(nBit, 1) = 1";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            feedback = new CandidateFeedbackViewModel
                            {
                                nID = Convert.ToInt32(dr["nID"]),

                                Que1 = dr["Que1"] != DBNull.Value
                                    ? dr["Que1"].ToString()
                                    : "",

                                Que2 = dr["Que2"] != DBNull.Value
                                    ? dr["Que2"].ToString()
                                    : "",

                                Que3 = dr["Que3"] != DBNull.Value
                                    ? dr["Que3"].ToString()
                                    : "",

                                Que4 = dr["Que4"] != DBNull.Value
                                    ? dr["Que4"].ToString()
                                    : "",

                                Que5 = dr["Que5"] != DBNull.Value
                                    ? dr["Que5"].ToString()
                                    : "",

                                sQue1 = "",
                                sQue2 = "",
                                sQue3 = "",
                                sQue4 = "",
                                sQue5 = "",

                                nOrgID = OrgID.Value,

                                nBit = dr["nBit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nBit"])
                                    : true,

                                nSABit = dr["nSABit"] != DBNull.Value
                                    ? Convert.ToBoolean(dr["nSABit"])
                                    : true
                            };
                        }
                    }
                }
            }

            if (feedback == null)
            {
                return NotFound("Feedback questions not found.");
            }

            return View(feedback);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateFeedback(
            CandidateFeedbackViewModel model)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? OrgID =
                HttpContext.Session.GetInt32("OrgID");

            if (OrgID == null || OrgID <= 0)
            {
                TempData["Error"] =
    "Organization session expired. Please login again.";

                return RedirectToAction("CreateFeedback", "Organization");
            }

            // =====================================================
            // VALIDATION
            // =====================================================

            if (string.IsNullOrEmpty(model.sQue1))
                ModelState.AddModelError(
                    "sQue1",
                    "Please select an answer.");

            if (string.IsNullOrEmpty(model.sQue2))
                ModelState.AddModelError(
                    "sQue2",
                    "Please select a rating.");

            if (string.IsNullOrEmpty(model.sQue3))
                ModelState.AddModelError(
                    "sQue3",
                    "Please select an answer.");

            if (string.IsNullOrEmpty(model.sQue4))
                ModelState.AddModelError(
                    "sQue4",
                    "Please select an emoji.");

            if (!ModelState.IsValid)
            {
                // IMPORTANT:
                // model is now CandidateFeedbackViewModel,
                // same type required by the View.
                return View(model);
            }

            // =====================================================
            // QUESTION 1
            // =====================================================

            int sQue1;

            if (model.sQue1 == "True")
            {
                sQue1 = 1;
            }
            else if (model.sQue1 == "False")
            {
                sQue1 = 0;
            }
            else
            {
                ModelState.AddModelError(
                    "sQue1",
                    "Invalid answer.");

                return View(model);
            }

            // =====================================================
            // QUESTION 2
            // =====================================================

            if (!int.TryParse(
                model.sQue2,
                out int sQue2))
            {
                ModelState.AddModelError(
                    "sQue2",
                    "Invalid rating.");

                return View(model);
            }

            // =====================================================
            // QUESTION 3
            // =====================================================

            int sQue3;

            if (model.sQue3 == "Yes")
            {
                sQue3 = 1;
            }
            else if (model.sQue3 == "No")
            {
                sQue3 = 0;
            }
            else
            {
                ModelState.AddModelError(
                    "sQue3",
                    "Invalid answer.");

                return View(model);
            }

            // =====================================================
            // QUESTION 4
            // =====================================================

            if (!int.TryParse(
                model.sQue4,
                out int sQue4))
            {
                ModelState.AddModelError(
                    "sQue4",
                    "Invalid emoji rating.");

                return View(model);
            }

            // =====================================================
            // QUESTION 5
            // =====================================================

            string sQue5 =
                model.sQue5 ?? "";

            // =====================================================
            // DATABASE
            // =====================================================

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO tblOrgFeedback
            (
                sQue1,
                sQue2,
                sQue3,
                sQue4,
                sQue5,
                nAdminID,
                RegDate,
                ModDate,
                nBit,
                nSABit
            )
            VALUES
            (
                @sQue1,
                @sQue2,
                @sQue3,
                @sQue4,
                @sQue5,
                @nAdminID,
                GETDATE(),
                GETDATE(),
                1,
                0
            )";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@sQue1",
                        SqlDbType.Int).Value = sQue1;

                    cmd.Parameters.Add(
                        "@sQue2",
                        SqlDbType.Int).Value = sQue2;

                    cmd.Parameters.Add(
                        "@sQue3",
                        SqlDbType.Int).Value = sQue3;

                    cmd.Parameters.Add(
                        "@sQue4",
                        SqlDbType.Int).Value = sQue4;

                    cmd.Parameters.Add(
                        "@sQue5",
                        SqlDbType.NVarChar,
                        200).Value =
                            string.IsNullOrWhiteSpace(sQue5)
                            ? DBNull.Value
                            : sQue5;

                    // Candidate ID
                    cmd.Parameters.Add(
                        "@nAdminID",
                        SqlDbType.Int).Value =
                            OrgID.Value;

                    con.Open();

                    int rows =
                        cmd.ExecuteNonQuery();

                    if (rows <= 0)
                    {
                        TempData["Error"] =
                            "Feedback was not saved.";

                        return View(model);
                    }
                }
            }

            TempData["Success"] = "Feedback submitted successfully.";

            return RedirectToAction(
                "CreateFeedback",
                "Organization");
        }


        [HttpGet]
        public IActionResult EditProfile()
        {
            int? orgId = HttpContext.Session.GetInt32("OrgID");

            if (orgId == null)
            {
                return RedirectToAction(
                    "OrganizationLogin",
                    "Account");
            }

            OrgProfileM? model =
                _accountRepository.GetOrganizationProfile(orgId.Value);

            if (model == null)
            {
                model = new OrgProfileM
                {
                    nOrgID = orgId.Value
                };
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(
    OrgProfileM model,
    IFormFile? CompanyLogo)
        {
            // PUT BREAKPOINT HERE

            int? orgId =
                HttpContext.Session.GetInt32("OrgID");

            if (orgId == null)
            {
                return RedirectToAction(
                    "OrganizationLogin",
                    "Account");
            }

            model.nOrgID = orgId.Value;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string logoPath =
                model.sCompanyLogo ?? string.Empty;

            if (CompanyLogo != null &&
                CompanyLogo.Length > 0)
            {
                string uploadsFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "organization");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string extension =
                    Path.GetExtension(
                        CompanyLogo.FileName)
                    .ToLowerInvariant();

                string[] allowedExtensions =
                {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "CompanyLogo",
                        "Only JPG, JPEG, PNG and WEBP files are allowed.");

                    return View(model);
                }

                if (CompanyLogo.Length >
                    5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "CompanyLogo",
                        "Company logo size cannot exceed 5 MB.");

                    return View(model);
                }

                string fileName =
                    Guid.NewGuid().ToString("N") +
                    extension;

                string filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using (FileStream stream =
                       new FileStream(
                           filePath,
                           FileMode.Create))
                {
                    CompanyLogo.CopyTo(stream);
                }

                logoPath = fileName;
            }

            // BREAKPOINT HERE
            bool result =
                _accountRepository.SaveOrganizationProfile(
                    model,
                    logoPath);

            if (!result)
            {
                TempData["ErrorMessage"] =
                    "Organization profile could not be updated.";

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Organization profile updated successfully.";

            return RedirectToAction("EditProfile");
        }



        // ==========================================
        // CANDIDATE LIST
        // ==========================================
        [HttpGet]
        public IActionResult TraineeList()
        {
            List<SATraineeListM> trainees = _repository.GetSATraineeList();
            return View(trainees);
        }

        // ==========================================
        // ORGANIZATION LIST
        // ==========================================
        [HttpGet]
        public IActionResult OrgList()
        {
            List<OrganizationUserM> organization = _repository.GetAllOrganizationList();
            return View(organization);
        }
    }
}
