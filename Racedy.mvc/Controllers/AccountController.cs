
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public AccountController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Displaying the login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Process login details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using var response = await _apiService.PostAsync(
                    "api/Auth/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        "", "Invalid email or password.");

                    return View(model);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<LoginResponse>();

                if (result == null ||
                    string.IsNullOrEmpty(result.Role))
                {
                    ModelState.AddModelError(
                        "", "Unable to read login response.");

                    return View(model);
                }

                // Save user information in MVC session
                HttpContext.Session.SetString(
                    "UserName", result.Name);

                HttpContext.Session.SetString(
                    "UserRole", result.Role);

                TempData["SuccessMessage"] =
                    "Welcome back, " + result.Name + "!";

                // Redirect Organisers to their dashboard
                if (result.Role.Equals(
                    "Organiser",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(
                        "Index", "Organiser");
                }

                // Participant dashboard will be added next
                if (result.Role.Equals(
                    "Participant",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(
                        "Index", "Home");
                }

                // Fallback for unrecognised roles
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "", "Cannot connect to RaceDay API.");

                return View(model);
            }
        }
    }

    public class LoginResponse
    {
        public string Message { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
