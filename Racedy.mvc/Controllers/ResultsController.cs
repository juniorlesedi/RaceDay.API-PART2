
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class ResultsController : Controller
    {
        private readonly RaceDayApiService _api;

        public ResultsController(RaceDayApiService api)
        {
            _api = api;
        }

        private IActionResult? CheckAccess()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrWhiteSpace(role))
                return RedirectToAction("Login", "Account");

            if (!role.Equals("Organiser",
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? eventId)
        {
            var access = CheckAccess();
            if (access != null) return access;

            ViewBag.SelectedEventId = eventId;
            ViewBag.Events = new List<RaceEventViewModel>();
            ViewBag.Registrations =
                new List<RegistrationViewModel>();

            try
            {
                var events = await _api
                    .GetAsync<List<RaceEventViewModel>>(
                        "api/Events");

                ViewBag.Events =
                    events ?? new List<RaceEventViewModel>();

                if (!eventId.HasValue || eventId.Value <= 0)
                    return View(new List<ResultViewModel>());

                var registrations = await _api
                    .GetAsync<List<RegistrationViewModel>>(
                        $"api/Registrations/event/{eventId.Value}");

                ViewBag.Registrations =
                    registrations ?? new List<RegistrationViewModel>();

                var results = await _api
                    .GetAsync<List<ResultViewModel>>(
                        $"api/Results/event/{eventId.Value}");

                return View(results ??
                    new List<ResultViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to load results. Check your API connection and permissions.";

                return View(new List<ResultViewModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int eventId, ResultInputViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (!ModelState.IsValid ||
                model.FinishTime <= TimeSpan.Zero)
            {
                TempData["ErrorMessage"] =
                    "Enter a valid registration, finish time and position.";

                return RedirectToAction(nameof(Index),
                    new { eventId });
            }

            try
            {
                using var response = await _api.PostAsync(
                    "api/Results", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Race result saved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        await ReadError(response);
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Cannot connect to RaceDay API.";
            }

            return RedirectToAction(nameof(Index),
                new { eventId });
        }

        private static async Task<string> ReadError(
            HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Your Organiser session has expired. Log in again.";

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return "You do not have permission to record this result.";

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                using var document = JsonDocument.Parse(content);

                if (document.RootElement.TryGetProperty(
                    "message", out var message))
                    return message.GetString() ?? "API error.";
            }
            catch (JsonException)
            {
            }

            return $"Unable to save result. API status: {(int)response.StatusCode}.";
        }
    }
}
