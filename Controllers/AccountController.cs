using HuntCV_Portal.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HuntCV_Portal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository;

        public AccountController(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
    }
}