using ERHuntCV.Models;
using ERHuntCV.Repositories;
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
        private readonly OrganizationRepository _organizationRepository;



        public OrganizationController(
            IConfiguration configuration,
            AccountRepository accountRepository,
            IWebHostEnvironment environment, AccountRepository repository, OrganizationRepository organizationRepository)
        {
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));

            _accountRepository = accountRepository
                ?? throw new ArgumentNullException(nameof(accountRepository));

            _environment = environment
                ?? throw new ArgumentNullException(nameof(environment));

            _repository = repository;

            _organizationRepository = organizationRepository
            ?? throw new ArgumentNullException(nameof(organizationRepository));
        }

        [HttpGet]
        [Route("Organization/Dashboard/{id?}")]
        [ResponseCache(
            NoStore = true,
            Location = ResponseCacheLocation.None)]
        public IActionResult Dashboard(string? id)
        {
            // =====================================================
            // GET ORGANIZATION ID FROM SESSION
            // =====================================================

            int? orgId = HttpContext.Session.GetInt32("OrgID");

            if (orgId == null)
            {
                return RedirectToAction(
                    "OrganizationLogin",
                    "Account");
            }

            // =====================================================
            // GET ORGANIZATION REGISTRATION DETAILS
            // =====================================================

            var organization =
                _repository.GetOrganizationRegistrationDetails(
                    orgId.Value);

            if (organization == null)
            {
                return NotFound(
                    "Organization registration not found.");
            }

            // =====================================================
            // CHECK REGISTRATION DATE
            // =====================================================

            if (organization.Value.RegDate == null)
            {
                return BadRequest(
                    "Organization Registration Date is missing.");
            }

            // =====================================================
            // CREATE ORGANIZATION CODE
            //
            // Example:
            // RegDate = 26/07/2026
            // nID     = 1
            //
            // Result = OR26072601
            // =====================================================

            string organizationCode =
                "OR" +
                organization.Value.RegDate.Value
                    .ToString("ddMMyy") +
                organization.Value.OrganizationID
                    .ToString("D2");

            // =====================================================
            // IF ID IS NOT PRESENT
            // REDIRECT TO CODE URL
            // =====================================================

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "Dashboard",
                    "Organization",
                    new
                    {
                        id = organizationCode
                    });
            }

            // =====================================================
            // CHECK ORGANIZATION CODE
            // =====================================================

            if (!string.Equals(
                    id,
                    organizationCode,
                    StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            // =====================================================
            // SESSION DATA
            // =====================================================

            ViewBag.OrgID =
                orgId.Value;

            ViewBag.OrgName =
                HttpContext.Session.GetString("OrgName");

            ViewBag.OrgEmail =
                HttpContext.Session.GetString("OrgEmail");

            ViewBag.OrgCode =
                organizationCode;

            // =====================================================
            // DASHBOARD COUNTS
            // =====================================================

            int traineeRegistrationCount = 0;
            int organizationRegistrationCount = 0;

            int internshipEligibleCount = 0;

            List<int> monthlyCandidateRegistrations =
                Enumerable.Repeat(0, 12).ToList();

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                // =================================================
                // TOTAL CANDIDATE & ORGANIZATION COUNT
                // =================================================

                string query = @"
            SELECT
                (SELECT COUNT(nID)
                 FROM tblCandidateReg)
                    AS CandidateCount,

                (SELECT COUNT(nID)
                 FROM tblOrgRegistration)
                    AS OrganizationCount;";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                using (SqlDataReader dr =
                       cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        traineeRegistrationCount =
                            Convert.ToInt32(
                                dr["CandidateCount"]);

                        organizationRegistrationCount =
                            Convert.ToInt32(
                                dr["OrganizationCount"]);
                    }
                }

                // =================================================
                // INTERNSHIP ELIGIBLE CANDIDATES
                // =================================================

                string eligibleQuery = @"
            SELECT COUNT(nID)
            FROM tblCandidateReg
            WHERE ISNULL(nBit, 1) = 1;";

                using (SqlCommand cmd =
                       new SqlCommand(
                           eligibleQuery,
                           con))
                {
                    object result =
                        cmd.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        internshipEligibleCount =
                            Convert.ToInt32(result);
                    }
                }

                // =================================================
                // MONTHLY CANDIDATE REGISTRATIONS
                // =================================================

                string monthlyQuery = @"
            SELECT
                MONTH(RegDate)
                    AS RegistrationMonth,

                COUNT(nID)
                    AS RegistrationCount

            FROM tblCandidateReg

            WHERE YEAR(RegDate) =
                  YEAR(GETDATE())

            GROUP BY
                MONTH(RegDate)

            ORDER BY
                MONTH(RegDate);";

                using (SqlCommand cmd =
                       new SqlCommand(
                           monthlyQuery,
                           con))
                using (SqlDataReader dr =
                       cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int month =
                            Convert.ToInt32(
                                dr["RegistrationMonth"]);

                        int count =
                            Convert.ToInt32(
                                dr["RegistrationCount"]);

                        if (month >= 1 &&
                            month <= 12)
                        {
                            monthlyCandidateRegistrations[
                                month - 1] = count;
                        }
                    }
                }
            }

            // =====================================================
            // GET TRAINEES
            // =====================================================

            List<SATraineeListM> trainees =
                _repository.GetAllTrainees();

            // =====================================================
            // GET ORGANIZATIONS
            // =====================================================

            List<OrganizationUserM> organizations =
                _repository.GetAllOrganizationList();

            // =====================================================
            // SEND DATA TO VIEW
            // =====================================================

            ViewBag.TraineeRegistrationCount =
                traineeRegistrationCount;

            ViewBag.OrganizationRegistrationCount =
                organizationRegistrationCount;

            ViewBag.Trainees =
                trainees;

            ViewBag.Organizations =
                organizations;

            ViewBag.InternshipEligibleCount =
                internshipEligibleCount;

            ViewBag.MonthlyCandidateRegistrations =
                monthlyCandidateRegistrations;

            // =====================================================
            // RETURN VIEW
            // =====================================================

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


        [HttpGet]
        public IActionResult CreatePostNew()
        {
            try
            {
                // Load dropdown/master data
                ViewBag.Positions =
                   _organizationRepository.GetPositions();

                ViewBag.Genders =
                    _organizationRepository.GetGenders();

                ViewBag.MinimumQualifications =
                   _organizationRepository.GetMinimumQualifications();

                ViewBag.InternshipTypes =
                   _organizationRepository.GetInternshipTypes();

                ViewBag.InternshipFellowshipTypes =
                   _organizationRepository.GetInternshipFellowshipTypes();

                ViewBag.TrainingInvolved =
                   _organizationRepository.GetTrainingInvolved();

                ViewBag.InternshipDurations =
                   _organizationRepository.GetInternshipDurations();

                ViewBag.InternshipModes =
                    _organizationRepository.GetInternshipModes();

                // Skill master data
                ViewBag.TechnicalSkills =
                    _organizationRepository.GetTechnicalSkills();

                ViewBag.MedicalSkills =
                  _organizationRepository.GetMedicalSkills();

                ViewBag.NonTechnicalSkills =
                   _organizationRepository.GetNonTechnicalSkills();

                return View(new OrgPostM());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(new OrgPostM());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePostNew(OrgPostM model)
        {
            try
            {
                int? orgId =
                    HttpContext.Session.GetInt32("OrgID");

                if (orgId == null || orgId <= 0)
                {
                    TempData["Error"] =
                        "Organization session expired. Please login again.";

                    return RedirectToAction(
                        "OrganizationLogin",
                        "Account");
                }

                model.nOrgID = orgId.Value;

                model.sMedicalSkills =
                    model.sMedicalSkills ?? "";

                model.sTechnicalSkills =
                    model.sTechnicalSkills ?? "";

                model.sNonTechnicalSkills =
                    model.sNonTechnicalSkills ?? "";

                model.sWorkingDays =
                    model.sWorkingDays ?? "";

                model.sFacilities =
                    model.sFacilities ?? "";


                if (!ModelState.IsValid)
                {
                    ViewBag.Positions =
                        _organizationRepository.GetPositions();

                    ViewBag.Genders =
                        _organizationRepository.GetGenders();

                    ViewBag.MinimumQualifications =
                        _organizationRepository.GetMinimumQualifications();

                    ViewBag.InternshipTypes =
                        _organizationRepository.GetInternshipTypes();

                    ViewBag.InternshipFellowshipTypes =
                        _organizationRepository.GetInternshipFellowshipTypes();

                    ViewBag.TrainingInvolved =
                        _organizationRepository.GetTrainingInvolved();

                    ViewBag.InternshipDurations =
                        _organizationRepository.GetInternshipDurations();

                    ViewBag.InternshipModes =
                        _organizationRepository.GetInternshipModes();

                    ViewBag.TechnicalSkills =
                        _organizationRepository.GetTechnicalSkills();

                    ViewBag.MedicalSkills =
                        _organizationRepository.GetMedicalSkills();

                    ViewBag.NonTechnicalSkills =
                        _organizationRepository.GetNonTechnicalSkills();

                    return View(model);
                }


                int postId =
                    _organizationRepository.CreateOrganizationPost(model);


                if (postId <= 0)
                {
                    TempData["Error"] =
                        "Post could not be created.";

                    return View(model);
                }


                TempData["Success"] =
                    "Internship post created successfully.";


                return RedirectToAction("PostDetails");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return View(model);
            }
        }

        // Master dropdowns

        [HttpGet]
        public IActionResult GetPosition()
        {
            return Json(_organizationRepository.GetPositions());
        }

        [HttpGet]
        public IActionResult GetGender()
        {
            return Json(_organizationRepository.GetGenders());
        }

        [HttpGet]
        public IActionResult GetMinimumQualification()
        {
            return Json(_organizationRepository.GetMinimumQualifications());
        }

        [HttpGet]
        public IActionResult GetInternshipType()
        {
            return Json(_organizationRepository.GetInternshipTypes());
        }

        [HttpGet]
        public IActionResult GetInternshipFellowshipType()
        {
            return Json(_organizationRepository.GetInternshipFellowshipTypes());
        }

        [HttpGet]
        public IActionResult GetTrainingInvolved()
        {
            return Json(_organizationRepository.GetTrainingInvolved());
        }

        [HttpGet]
        public IActionResult GetInternshipDuration()
        {
            return Json(_organizationRepository.GetInternshipDurations());
        }

        [HttpGet]
        public IActionResult GetInternshipMode()
        {
            return Json(_organizationRepository.GetInternshipModes());
        }

        // =========================================================
        // UPDATE LOCATION
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePostLocation(
            OrgPostM model)
        {
            string? sessionOrgId =
                HttpContext.Session.GetString("OrgID");

            if (string.IsNullOrWhiteSpace(sessionOrgId))
            {
                return RedirectToAction("Login", "Account");
            }

            int orgId =
                Convert.ToInt32(sessionOrgId);


            // Save location IDs
            _organizationRepository
                .UpdateOrganizationLocation(
                    model.nID,
                    orgId,
                    model.CountryID,
                    model.StateID,
                    model.CityID);


            TempData["Success"] =
                "Location updated successfully.";


            return RedirectToAction(
                "EditPost",
                new
                {
                    id = model.nID
                });
        }


        [HttpGet]
        public IActionResult GetTechnicalSkills()
        {
            var skills = _organizationRepository.GetTechnicalSkills();
            return Json(skills);
        }

        [HttpGet]
        public IActionResult GetMedicalSkills()
        {
            var skills = _organizationRepository.GetMedicalSkills();
            return Json(skills);
        }

        [HttpGet]
        public IActionResult GetNonTechnicalSkills()
        {
            var skills = _organizationRepository.GetNonTechnicalSkills();
            return Json(skills);
        }


    }
}
