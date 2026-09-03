using ErJobPortal.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HuntCV_Portal.Controllers
{
    public class SALoginController : Controller
    {
        private readonly AccountRepository _repository;

        public SALoginController(
            AccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SALoginM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SALoginM? user =
                _repository.Login(
                    model.sEmail,
                    model.sPassword);

            if (user != null)
            {
                HttpContext.Session.SetInt32(
                    "SAID",
                    user.SAID);



                HttpContext.Session.SetString(
                    "SAName",
                    user.sFName ?? "");



                HttpContext.Session.SetString(
                    "SARole",
                    user.sRole ?? "");

                return RedirectToAction(
                    "Dashboard",
                    "SuperAdmin");
            }

            ViewBag.Error =
                "Invalid Email or Password.";

            return View(model);
        }
    }
}