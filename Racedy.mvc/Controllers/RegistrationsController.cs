
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly RaceDayApiService _api;

        public RegistrationsController(RaceDayApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? eventId)
        {
            string? role =
                HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            if (role != "Organiser")
                return StatusCode(403);

            try
            {
                var events = await _api
                    .GetAsync<List<RaceEventViewModel>>(
                        "api/Events");

                ViewBag.Events =
                    events ?? new List<RaceEventViewModel>();

                ViewBag.SelectedEventId = eventId;

                if (!eventId.HasValue || eventId.Value <= 0)
                    return View(new List<RegistrationViewModel>());

                var registrations = await _api
                    .GetAsync<List<RegistrationViewModel>>(
                        $"api/Registrations/event/{eventId.Value}");

                return View(registrations ??
                    new List<RegistrationViewModel>());
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage =
                    "Unable to load enrolments. Please check your API connection and Organiser session.";

                ViewBag.Events =
                    new List<RaceEventViewModel>();

                return View(new List<RegistrationViewModel>());
            }
        }
    }
}
