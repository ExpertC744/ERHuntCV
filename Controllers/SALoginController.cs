using ErJobPortal.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HuntCV_Portal.Controllers
{
    public class SALoginController : Controller
    {
        private readonly AccountRepository _repository;

        public SALoginController(AccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(SALoginM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _repository.Login(model.sEmail, model.sPassword);

            if (user != null)
            {
                // Store Super Admin details in session
                HttpContext.Session.SetString(
                    "SAID",
                    user.nID.ToString()
                );

                HttpContext.Session.SetString(
                    "SAName",
                    user.sFName
                );

                HttpContext.Session.SetString(
                    "SARole",
                    user.sRole
                );

                // Pass nID to Dashboard URL
                return RedirectToAction(
                    "Dashboard",
                    "SuperAdmin",
                    new { id = user.nID }
                );
            }

            ViewBag.Error = "Invalid Email or Password";
            return View(model);
        }
    }
}