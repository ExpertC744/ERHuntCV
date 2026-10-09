using ERHuntCV.Models;
using ERHuntCV.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ERHuntCV.Controllers
{
    public class HomeController : Controller
    {

        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(
    ILogger<HomeController> logger,
    IConfiguration configuration,
    EmailService emailService)
        {
            _logger = logger;

            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));

            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpGet]
        public IActionResult Index()
        {
            int traineeCount = 0;
            int organizationCount = 0;
            int postCount = 0;
            int stateCount = 0;
            int countryCount = 0;

            string? connectionString =
       _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // =====================================================
                // TRAINEE, ORGANIZATION AND POST COUNT
                // =====================================================

                string query = @"
     SELECT
         (SELECT COUNT(nID)
          FROM tblCandidateReg) AS CandidateCount,

         (SELECT COUNT(nID)
          FROM tblOrgRegistration) AS OrganizationCount,

         (SELECT COUNT(nID)
          FROM tblPost) AS PostCount;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        traineeCount =
                            Convert.ToInt32(dr["CandidateCount"]);

                        organizationCount =
                            Convert.ToInt32(dr["OrganizationCount"]);

                        postCount =
                            Convert.ToInt32(dr["PostCount"]);
                    }
                }

                // =====================================================
                // STATE COUNT
                // =====================================================

                using (SqlCommand cmdState =
                       new SqlCommand("SP_CountState", con))
                {
                    cmdState.CommandType = CommandType.StoredProcedure;

                    object? result = cmdState.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        stateCount = Convert.ToInt32(result);
                    }
                }

                // =====================================================
                // COUNTRY COUNT
                // =====================================================

                using (SqlCommand cmdCountry =
                       new SqlCommand("SP_CountCountry", con))
                {
                    cmdCountry.CommandType = CommandType.StoredProcedure;

                    object? result = cmdCountry.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        countryCount = Convert.ToInt32(result);
                    }
                }
            }

            // =====================================================
            // SEND COUNTS TO VIEW
            // =====================================================

            ViewBag.TraineeCount = traineeCount;
            ViewBag.OrganizationCount = organizationCount;
            ViewBag.PostCount = postCount;
            ViewBag.StateCount = stateCount;
            ViewBag.CountryCount = countryCount;

            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //khushi 19/09/26
        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ContactUs(ViewModelTrainee rg)
        {
            try
            {
                // ==========================================
                // VALIDATION
                // ==========================================

                if (rg == null)
                {
                    TempData["ContactUs"] =
                        "Please enter valid contact details.";

                    return RedirectToAction("Contact");
                }

                if (string.IsNullOrWhiteSpace(rg.FullName) ||
                    string.IsNullOrWhiteSpace(rg.Email) ||
                    string.IsNullOrWhiteSpace(rg.Subject) ||
                    string.IsNullOrWhiteSpace(rg.Description))
                {
                    TempData["ContactUs"] =
                        "Please fill all required fields.";

                    return RedirectToAction("Contact");
                }


                // ==========================================
                // SAVE CONTACT DATA
                // ==========================================

                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    await con.OpenAsync();

                    string query = @"
                INSERT INTO tblContactUs
                (
                    FullName,
                    Email,
                    MobileNo,
                    Subject,
                    Description
                )
                VALUES
                (
                    @FullName,
                    @Email,
                    @MobileNo,
                    @Subject,
                    @Description
                )";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@FullName",
                            (object?)rg.FullName?.Trim()
                            ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Email",
                            (object?)rg.Email?.Trim()
                            ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@MobileNo",
                            (object?)rg.MobileNo?.Trim()
                            ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Subject",
                            (object?)rg.Subject?.Trim()
                            ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Description",
                            (object?)rg.Description?.Trim()
                            ?? DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }


                // ==========================================
                // SEND EMAIL TO THE EMAIL ENTERED BY USER
                // ==========================================

                try
                {
                    await _emailService.SendContactThankYouEmailAsync(
                        rg.FullName,
                        rg.Email,
                        rg.Subject,
                        rg.Description);

                    _logger.LogInformation(
                        "Contact thank-you email sent successfully to {Email}",
                        rg.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(
                        emailEx,
                        "Contact saved but thank-you email failed. Recipient: {Email}",
                        rg.Email);

                    TempData["ContactUs"] =
                        "Your message was saved, but the confirmation email could not be sent. " +
                        "Please check the email address or try again.";

                    return RedirectToAction("Contact");
                }


                // ==========================================
                // SUCCESS
                // ==========================================

                ModelState.Clear();

                TempData["ContactUs"] =
                    "Thank you for contacting us! " +
                    "Your message has been sent successfully.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while saving Contact Us form.");

                TempData["ContactUs"] =
                    "Something went wrong. Please try again.";

                return RedirectToAction("Contact");
            }
        }


    }
}
