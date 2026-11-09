
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public EventsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        private IActionResult? CheckAccess()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            if (!role.Equals("Organiser",
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            return null;
        }

        // READ - Display events
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                var events = await _apiService
                    .GetAsync<List<RaceEventViewModel>>(
                        "api/Events");

                return View(events ?? new List<RaceEventViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to load events from the API.";

                return View(new List<RaceEventViewModel>());
            }
        }

        // CREATE - Display form
        [HttpGet]
        public IActionResult Create()
        {
            var access = CheckAccess();
            if (access != null) return access;

            return View(new EventInputViewModel());
        }

        // CREATE - Save event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EventInputViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var response = await _apiService
                    .PostAsync("api/Events", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event created successfully!";

                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("",
                    await ReadError(response));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Cannot connect to RaceDay API.");
            }

            return View(model);
        }

        // EDIT - Display existing event
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                var raceEvent = await _apiService
                    .GetAsync<RaceEventViewModel>(
                        $"api/Events/{id}");

                if (raceEvent == null)
                    return NotFound();

                var model = new EventInputViewModel
                {
                    Name = raceEvent.name,
                    Description = raceEvent.description,
                    EventDate = raceEvent.event_date,
                    Location = raceEvent.location,
                    DistanceKm = raceEvent.Distance_km,
                    EventType = raceEvent.event_type
                };

                ViewBag.EventId = id;
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to load the selected event.";

                return RedirectToAction(nameof(Index));
            }
        }

        // EDIT - Save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, EventInputViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            ViewBag.EventId = id;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var response = await _apiService
                    .PutAsync($"api/Events/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event updated successfully!";

                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("",
                    await ReadError(response));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Cannot connect to RaceDay API.");
            }

            return View(model);
        }

        // DELETE - Remove event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                using var response = await _apiService
                    .DeleteAsync($"api/Events/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event deleted successfully!";
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

            return RedirectToAction(nameof(Index));
        }

        private static async Task<string> ReadError(
            HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Your Organiser API session has expired. Please log in again.";

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return "You do not have permission to change this event.";

            var content = await response.Content
                .ReadAsStringAsync();

            return string.IsNullOrWhiteSpace(content)
                ? $"API error: {(int)response.StatusCode}"
                : content;
        }
    }
}
