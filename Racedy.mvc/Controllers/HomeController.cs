
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public HomeController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display the Home dashboard
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");

            // Redirect to login if not logged in
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "Account");
            }

            // Redirect Organisers to their dashboard
            if (userRole.Equals(
                "Organiser",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Organiser");
            }

            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            return View();
        }

        // Test whether the API recognises the login session
        public async Task<IActionResult> TestApiSession()
        {
            if (string.IsNullOrEmpty(
                HttpContext.Session.GetString("UserRole")))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var result = await _apiService.GetAsync<object>(
                    "api/Auth/me");

                return Json(result);
            }
            catch (HttpRequestException)
            {
                return Content("API session test failed.");
            }
        }
    }
}


