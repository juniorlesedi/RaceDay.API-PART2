
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Organiser")
            {
                return StatusCode(403, "Access denied.");
            }

            ViewBag.UserName =
                HttpContext.Session.GetString("UserName");

            return View();
        }
    }
}

