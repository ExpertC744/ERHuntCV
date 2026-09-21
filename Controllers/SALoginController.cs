using ERHuntCV.Models;
using ERHuntCV.Services;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HuntCV_Portal.Controllers
{
    public class SALoginController : Controller
    {
        private readonly AccountRepository _repository;
        private readonly EmailService _emailService;

        public SALoginController(
            AccountRepository repository,
            EmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }
        // shrirang 18/09/26
        // GET: /SALogin
        [HttpGet]
        public IActionResult Index()
        {
            return View(new SALoginM());
        }

        // POST: /SALogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SALoginM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _repository.Login(
                model.sEmail,
                model.sPassword
            );

            if (user != null)
            {
                HttpContext.Session.SetString(
                    "SAID",
                    user.nID.ToString()
                );

                HttpContext.Session.SetString(
                    "SAName",
                    user.sFName ?? ""
                );

                HttpContext.Session.SetString(
                    "SARole",
                    user.sRole ?? ""
                );

                return RedirectToAction(
                    "Dashboard",
                    "SuperAdmin",
                    new { id = user.nID }
                );
            }

            ViewBag.Error = "Invalid Email or Password";

            return View(model);
        }

        // GET: /SALogin/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new SAForgotPasswordModel());
        }

        // POST: /SALogin/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            SAForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user =
                    _repository.GetSuperAdminByEmail(model.sEmail);

                if (user == null)
                {
                    ViewBag.Error =
                        "No active Super Admin account was found with this email address.";

                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(user.sPassword))
                {
                    ViewBag.Error =
                        "Password information is not available for this account.";

                    return View(model);
                }

                await _emailService.SendSuperAdminLoginDetailsAsync(
                    user.sEmail,
                    user.sPassword
                );

                TempData["Success"] =
                    "Your login credentials have been sent successfully to your registered email address.";

                return RedirectToAction(
                    "ForgotPassword",
                    "SALogin"
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Email sending failed: " + ex.Message;

                return View(model);
            }
        }

        //// GET: /SALogin/ResetPassword
        //[HttpGet]
        //public IActionResult ResetPassword(string email)
        //{
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        return RedirectToAction(
        //            "ForgotPassword",
        //            "SALogin"
        //        );
        //    }

        //    var model = new SuperAdminResetPassword
        //    {
        //        sEmail = email
        //    };

        //    return View(model);
        //}

        //// POST: /SALogin/ResetPassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ResetPassword(
        //    SuperAdminResetPassword model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var updated =
        //        _repository.ResetSuperAdminPassword(
        //            model.sEmail,
        //            model.NewPassword
        //        );

        //    if (!updated)
        //    {
        //        ViewBag.Error =
        //            "Unable to reset password. Please try again.";

        //        return View(model);
        //    }

        //    TempData["Success"] =
        //        "Password reset successfully. Please login with your new password.";

        //    return RedirectToAction(
        //        "Index",
        //        "SALogin"
        //    );
        //}

        // =========================================================
        // RESET PASSWORD - GET
        // =========================================================
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View(new SuperAdminResetPassword());
        }


        // =========================================================
        // RESET PASSWORD - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(
            SuperAdminResetPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // =====================================================
                // CHECK EMAIL EXISTS
                // =====================================================

                var user =
                    _repository.GetSuperAdminByEmail(model.sEmail);

                if (user == null)
                {
                    ModelState.AddModelError(
                        "sEmail",
                        "No active Super Admin account was found with this email address."
                    );

                    return View(model);
                }

                // =====================================================
                // RESET PASSWORD
                // =====================================================

                bool updated =
                    _repository.ResetSuperAdminPassword(
                        model.sEmail,
                        model.NewPassword
                    );

                if (!updated)
                {
                    ViewBag.Error =
                        "Unable to reset password. Please try again.";

                    return View(model);
                }

                // =====================================================
                // SUCCESS
                // =====================================================

                TempData["Success"] =
                    "Password reset successfully. Please login with your new password.";

                return RedirectToAction(
                    "Index",
                    "SALogin"
                );
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