using ERHuntCV.Models;
using ERHuntCV.Repositories;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace HuntCV_Portal.Controllers
{
    public class CandidateController : Controller
    {
        private readonly CandidateProfileRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly AccountRepository _repository;

        public CandidateController(IConfiguration configuration, AccountRepository repository, CandidateProfileRepository repo)
        {
            _configuration = configuration;
            _repository = repository;
            _repo = repo;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Dashboard(string? id)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // =========================================================
            // GET CANDIDATE REGISTRATION DETAILS
            // =========================================================

            var candidate =
                _repository.GetCandidateRegistrationDetails(
                    candidateId.Value);

            if (candidate == null)
            {
                return NotFound("Candidate registration not found.");
            }

            // =========================================================
            // CHECK DOB AND REGISTRATION DATE
            // =========================================================

            if (candidate.Value.DOB == null ||
                candidate.Value.RegDate == null)
            {
                return BadRequest(
                    "Candidate DOB or Registration Date is missing.");
            }

            // =========================================================
            // CREATE CANDIDATE CODE
            // =========================================================
            //
            // CD
            // + DOB ddMMyy
            // + Registration Date MMdd
            // + Candidate ID 2 digits
            //
            // Example:
            // DOB      = 08/05/1999
            // RegDate  = 26/07/2026
            // nID      = 1
            //
            // CD080599072601
            // =========================================================

            string candidateCode =
                "CD" +
                candidate.Value.DOB.Value.ToString("ddMMyy") +
                candidate.Value.RegDate.Value.ToString("MMdd") +
                candidate.Value.CandidateID.ToString("D2");

            // =========================================================
            // IF NORMAL URL IS OPENED
            // REDIRECT TO CODE URL
            // =========================================================

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(
                    "Dashboard",
                    "Candidate",
                    new { id = candidateCode });
            }

            // =========================================================
            // OPTIONAL: CHECK THAT URL CODE BELONGS TO LOGGED-IN
            // CANDIDATE
            // =========================================================

            if (id != candidateCode)
            {
                return NotFound();
            }

            // =========================================================
            // EXISTING DASHBOARD CODE
            // =========================================================

            ViewBag.CandidateID =
                candidateId;

            ViewBag.CandidateName =
                HttpContext.Session.GetString(
                    "CandidateName");

            ViewBag.CandidateEmail =
                HttpContext.Session.GetString(
                    "CandidateEmail");

            ViewBag.CandidateCode =
                candidateCode;

            // =========================================================
            // ORGANIZATION COUNT
            // =========================================================

            int organizationRegistrationCount = 0;

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT COUNT(nID)
            FROM tblOrgRegistration;";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    organizationRegistrationCount =
                        Convert.ToInt32(
                            cmd.ExecuteScalar());
                }
            }

            // =========================================================
            // ORGANIZATION LIST
            // =========================================================

            List<OrganizationUserM> organizations =
                _repository.GetAllOrganizationList();

            ViewBag.OrganizationRegistrationCount =
                organizationRegistrationCount;

            ViewBag.Organizations =
                organizations;

            return View();
        }

        // =========================================================
        // CANDIDATE CREATE FEEDBACK - GET
        // =========================================================
        [HttpGet]
        public IActionResult CreateFeedback()

        {
            // =====================================================
            // GET CANDIDATE ID
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
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
            FROM tblSATRFeedback
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

                                nCandidateID = candidateId.Value,

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





        // =========================================================
        // CANDIDATE FEEDBACK - CREATE - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateFeedback(
            CandidateFeedbackViewModel model)
        {
            // =====================================================
            // GET CANDIDATE ID FROM SESSION
            // =====================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null || candidateId <= 0)
            {
                TempData["Error"] =
                    "Candidate session expired. Please login again.";

                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
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
            INSERT INTO tblCandidateFeedback
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
                            candidateId.Value;

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

            TempData["Success"] =
                "Feedback submitted successfully.";

            return RedirectToAction(
                "CreateFeedback");
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

        // =========================================================
        // GET PROFILE
        // =========================================================

        [HttpGet]
        public IActionResult Profile()
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            CandidateProfileModel? profile = _repo.GetProfile(candidateId.Value);

            if (profile == null)
            {
                profile = new CandidateProfileModel
                {
                    CandidateID = candidateId.Value
                };
            }

            CandidateProfileViewModel vm = LoadProfileDropdowns(profile);

            return View(vm);
        }




        // =========================================================
        // LOAD ALL DROPDOWNS
        // =========================================================

        private CandidateProfileViewModel LoadProfileDropdowns(CandidateProfileModel profile)
        {
            return new CandidateProfileViewModel
            {
                Profile = profile,

                Divisions = _repo.GetDivisions(),
                Streams = _repo.GetStreams(),
                GraduationStatuses = _repo.GetGraduationStatuses(),
                InternshipFellowshipType = _repo.GetInternshipFellowshipType(),
                InternshipTitles = _repo.GetInternshipTitles(),
                InternshipDurations = _repo.GetInternshipDurations(),
                InternshipStatuses = _repo.GetInternshipStatuses(),
                Relationships = _repo.GetRelationships()
            };
        }



        // =====================================================
        // UPDATE ADDRESS
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(CandidateProfileViewModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account"
                );
            }

            _repo.UpdateAddress(
                candidateId.Value,
                model.Profile.CountryID,
                model.Profile.StateID,
                model.Profile.CityID,
                model.Profile.Pincode
            );
            TempData["Success"] = "Address updated successfully.";
            return RedirectToAction("Profile");
        }



        // =====================================================
        // UPDATE EDUCATION
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEducation(
    [Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            // TEMPORARY TEST
            Console.WriteLine("CandidateID = " + model.CandidateID);
            Console.WriteLine("SSC_YEAR = " + model.SSC_YEAR);
            Console.WriteLine("SSC_DIVISION = " + model.SSC_DIVISION);
            Console.WriteLine("Graduation_Year = " + model.Graduation_Year);
            Console.WriteLine("PG_Year = " + model.PG_Year);
            Console.WriteLine("PhD_Year = " + model.PhD_Year);

            _repo.UpdateEducation(model);

            TempData["Success"] = "Education updated successfully.";

            return RedirectToAction("Profile");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateReferences([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateReferences(model);

            TempData["Success"] = "References updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE ACHIEVEMENTS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAchievements([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateAchievements(model);

            TempData["Success"] = "Achievements updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE LINKS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLinks([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateLinks(model);

            TempData["Success"] = "Links updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE OBJECTIVE
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateObjective(
            [Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateObjective(model);

            TempData["Success"] = "Objective updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE SKILLS
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSkills([Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId = HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateSkills(model);

            TempData["Success"] = "Skills updated successfully.";

            return RedirectToAction("Profile");
        }


        // =========================================================
        // UPDATE DOCUMENTS / HOBBIES
        // =========================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateDocuments([Bind(Prefix = "Profile")] CandidateProfileModel model)
        //{
        //    int? candidateId = HttpContext.Session.GetInt32("CandidateID");

        //    if (candidateId == null)
        //    {
        //        return RedirectToAction("CandidateLogin", "Account");
        //    }

        //    model.CandidateID = candidateId.Value;

        //    _repo.UpdateDocuments(model);

        //    TempData["Success"] = "Documents and personal details updated successfully.";

        //    return RedirectToAction("Profile");
        //}




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLanguages(
    [Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            // Always use CandidateID from Session
            model.CandidateID = candidateId.Value;

            // Save languages
            _repo.UpdateLanguages(model);

            TempData["Success"] =
                "Languages updated successfully.";

            return RedirectToAction("Profile");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInternshipDetails(
    [Bind(Prefix = "Profile")] CandidateProfileModel model)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }

            model.CandidateID = candidateId.Value;

            _repo.UpdateInternshipDetails(model);

            TempData["Success"] =
                "Internship details updated successfully.";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDocuments(
     CandidateProfileModel model,
     IFormFile? ResumeFile,
     IFormFile? PhotoFile,
     IFormFile? SignatureFile)
        {
            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId == null)
            {
                return RedirectToAction("CandidateLogin", "Account");
            }

            model.CandidateID = candidateId.Value;

            // Save files here

            return RedirectToAction("Profile");
        }
    }
}
