using ERHuntCV.Models;
using ERHuntCV.Services;
using HuntCV_Portal.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;




namespace HuntCV_Portal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly EmailService _emailService;

        public AccountController(
            AccountRepository accountRepository,
            EmailService emailService)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult OrganizationLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationLogin(OrganizationLoginM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OrganizationRegisterM? organization =
                _accountRepository.OrganizationLogin(model);

            if (organization == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }

            HttpContext.Session.SetInt32(
                "OrgID",
                organization.nID);

            HttpContext.Session.SetString(
                "OrgName",
                organization.sOrgName ?? "");

            HttpContext.Session.SetString(
                "OrgEmail",
                organization.sEmail ?? "");

            return RedirectToAction(
                "Dashboard",
                "Organization");
        }

        [HttpGet]
        public IActionResult OrganizationRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationRegister(OrganizationRegisterM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                int result = _accountRepository.RegisterOrganization(model);

                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Organization registered successfully.";

                    return RedirectToAction("OrganizationLogin", "Account");
                }

                ModelState.AddModelError(
                    "",
                    "Organization registration failed."
                );

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "An error occurred: " + ex.Message
                );

                return View(model);
            }
        }



        // =========================
        // Candidate Login
        // =========================
        [HttpGet]
        public IActionResult CandidateLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateLogin(CandidateLoginM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CandidateRegisterM? candidate =
                _accountRepository.LoginCandidate(model);

            if (candidate == null)
            {
                ViewBag.Error =
                    "Invalid email or password.";

                return View(model);
            }

            HttpContext.Session.SetInt32(
                "CandidateID",
                candidate.nID);

            HttpContext.Session.SetString(
                "CandidateName",
                candidate.sFName ?? "");

            HttpContext.Session.SetString(
                "CandidateEmail",
                candidate.sEmail ?? "");

            return RedirectToAction(
                "Dashboard",
                "Candidate");
        }

        // =========================
        // Candidate Registration
        // =========================
        [HttpGet]
        public IActionResult CandidateRegistration()
        {
            return View();
        }


        // =========================
        // Candidate Registration POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateRegistration(CandidateRegisterM model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                // ==========================================
                // PROFILE IMAGE UPLOAD
                // ==========================================

                if (model.ProfileImageFile != null &&
                    model.ProfileImageFile.Length > 0)
                {
                    // ==========================================
                    // ALLOWED IMAGE TYPES
                    // ==========================================

                    string[] allowedExtensions =
                    {
    ".png",
    ".jpg",
    ".jpeg"
};

                    string extension =
                        Path.GetExtension(
                            model.ProfileImageFile.FileName
                        ).ToLowerInvariant();

                    // ==========================================
                    // CHECK IMAGE EXTENSION
                    // ==========================================

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Only PNG, JPG and JPEG images are allowed."
                        );

                        return View(model);
                    }

                    // ==========================================
                    // MAXIMUM 5 MB
                    // ==========================================

                    if (model.ProfileImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Image size must be less than 5 MB."
                        );

                        return View(model);
                    }

                    // ==========================================
                    // CREATE UPLOAD FOLDER
                    // ==========================================

                    string uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "candidates"
                    );

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // ==========================================
                    // GENERATE IMAGE NAME: 1, 2, 3, 4...
                    // ==========================================

                    int nextNumber = 1;

                    while (Directory.GetFiles(
                        uploadFolder,
                        nextNumber + ".*"
                    ).Length > 0)
                    {
                        nextNumber++;
                    }

                    // Example: 1.jpg, 2.png, 3.jpeg
                    string imageName =
                        nextNumber + extension;

                    // ==========================================
                    // SAVE IMAGE
                    // ==========================================

                    string filePath = Path.Combine(
                        uploadFolder,
                        imageName
                    );

                    using (FileStream stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        model.ProfileImageFile.CopyTo(stream);
                    }

                    // ==========================================
                    // ONLY IMAGE NAME GOES TO DATABASE
                    // ==========================================

                    model.sProfileImage = imageName;
                }
                else
                {
                    model.sProfileImage = "";
                }

                int candidateID =
                    _accountRepository.Register(model);

                if (candidateID > 0)
                {
                    TempData["SuccessMessage"] =
                        "Candidate registration successful.";

                    return RedirectToAction(
                        "CandidateLogin",
                        "Account");
                }

                ModelState.AddModelError(
                    "",
                    "Candidate registration failed.");

                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "An error occurred while registering candidate.");

                return View(model);
            }
        }

        // shrirang 18/09/26 -- candidate
        // =========================================================
        // CANDIDATE FORGOT PASSWORD
        // =========================================================

        [HttpGet]
        public IActionResult CandidateForgotPassword()
        {
            return View(new CandidateForgotPassword());
        }


        // =========================================================
        // CANDIDATE FORGOT PASSWORD - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CandidateForgotPassword(
            CandidateForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.sEmail.Trim();

                // ==========================================
                // GET CANDIDATE
                // ==========================================

                CandidateRegisterM? candidate =
                    _accountRepository.GetCandidateByEmail(email);

                if (candidate == null)
                {
                    ViewBag.Error =
                        "No candidate account was found with this email address.";

                    return View(model);
                }

                // ==========================================
                // CHECK EMAIL
                // ==========================================

                if (string.IsNullOrWhiteSpace(candidate.sEmail))
                {
                    ViewBag.Error =
                        "Candidate email address is empty in database.";

                    return View(model);
                }

                // ==========================================
                // CHECK PASSWORD
                // ==========================================

                if (string.IsNullOrWhiteSpace(candidate.sPassword))
                {
                    ViewBag.Error =
                        "Password information is not available for this candidate account.";

                    return View(model);
                }

                // ==========================================
                // USE ACTUAL EMAIL FROM DATABASE
                // ==========================================

                string candidateEmail =
                    candidate.sEmail.Trim();

                // ==========================================
                // SEND EMAIL
                // ==========================================

                await _emailService.SendCandidateLoginDetailsAsync(
                    candidateEmail,
                    candidate.sPassword);

                // ==========================================
                // SUCCESS
                // ==========================================

                TempData["Success"] =
                    "Your login details have been sent to your registered email address. Please check your inbox and spam folder.";

                return RedirectToAction(
                    "CandidateForgotPassword",
                    "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Email sending failed: " + ex.Message;

                return View(model);
            }
        }

        // shrirang 18/09/26

        //// =========================================================
        //// CANDIDATE RESET PASSWORD - GET
        //// =========================================================

        [HttpGet]
        public IActionResult CandidateResetPassword()
        {
            return View(new CandidateResetPassword());
        }


        // =========================================================
        // CANDIDATE RESET PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateResetPassword(
      CandidateResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                CandidateRegisterM? candidate =
                    _accountRepository.GetCandidateByEmail(
                        model.sEmail);

                if (candidate == null)
                {
                    ModelState.AddModelError(
                        "sEmail",
                        "No candidate account was found with this email address.");

                    return View(model);
                }

                bool updated =
                    _accountRepository.ResetCandidatePassword(
                        model.sEmail,
                        model.NewPassword);

                if (!updated)
                {
                    ViewBag.Error =
                        "Unable to reset password. Please try again.";

                    return View(model);
                }

                TempData["Success"] =
                    "Password reset successfully. Please login with your new password.";

                return RedirectToAction(
                    "CandidateLogin",
                    "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Password reset failed: " + ex.Message;

                return View(model);
            }
        }



        // =========================================================
        // ORGANIZATION FORGOT PASSWORD - GET
        // =========================================================

        [HttpGet]
        public IActionResult OrganizationForgotPassword()
        {
            return View(new OrganizationForgotPassword());
        }


        // =========================================================
        // ORGANIZATION FORGOT PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrganizationForgotPassword(
            OrganizationForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.sEmail.Trim();

                // ==========================================
                // CHECK ORGANIZATION EMAIL IN DATABASE
                // ==========================================

                OrganizationRegisterM? organization =
                    _accountRepository.GetOrganizationByEmail(email);

                if (organization == null)
                {
                    ViewBag.Error =
                        "No organization account was found with this email address.";

                    return View(model);
                }

                // ==========================================
                // CHECK PASSWORD
                // ==========================================

                if (string.IsNullOrWhiteSpace(
                    organization.sPassword))
                {
                    ViewBag.Error =
                        "Password information is not available for this organization account.";

                    return View(model);
                }

                // ==========================================
                // GET ACTUAL EMAIL FROM DATABASE
                // ==========================================

                string organizationEmail =
                    organization.sEmail?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(organizationEmail))
                {
                    ViewBag.Error =
                        "Organization email address is empty in database.";

                    return View(model);
                }

                // ==========================================
                // SEND LOGIN CREDENTIALS
                // ==========================================

                await _emailService.SendOrganizationLoginDetailsAsync(
                    organizationEmail,
                    organization.sPassword);

                // ==========================================
                // SUCCESS
                // ==========================================

                TempData["Success"] =
                    "Your login details have been sent successfully to your registered email address. Please check your inbox and spam folder.";

                return RedirectToAction(
                    "OrganizationForgotPassword",
                    "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Email sending failed: " + ex.Message;

                return View(model);
            }
        }


        // org

        // ============================================================
        // ORGANIZATION RESET PASSWORD - GET
        // ============================================================

        [HttpGet]
        public IActionResult OrganizationResetPassword()
        {
            return View(new OrganizationResetPassword());
        }


        // ============================================================
        // ORGANIZATION RESET PASSWORD - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationResetPassword(
            OrganizationResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string email = model.sEmail.Trim();

                // Check whether organization exists
                OrganizationRegisterM? organization =
                    _accountRepository.GetOrganizationByEmail(email);

                if (organization == null)
                {
                    ModelState.AddModelError(
                        "sEmail",
                        "No organization account was found with this email address.");

                    return View(model);
                }

                // Reset password
                bool updated =
                    _accountRepository.ResetOrganizationPassword(
                        email,
                        model.NewPassword);

                if (!updated)
                {
                    ViewBag.Error =
                        "Unable to reset password. Please try again.";

                    return View(model);
                }

                TempData["Success"] =
                    "Password reset successfully. Please login with your new password.";

                return RedirectToAction(
                    "OrganizationLogin",
                    "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Password reset failed: " + ex.Message;

                return View(model);
            }
        }

    }
}