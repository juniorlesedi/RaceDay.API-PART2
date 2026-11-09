
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public CategoriesController(RaceDayApiService apiService)
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                var categories = await _apiService
                    .GetAsync<List<CategoryViewModel>>(
                        "api/Categories");

                var events = await _apiService
                    .GetAsync<List<RaceEventViewModel>>(
                        "api/Events");

                ViewBag.EventNames = (events ??
                    new List<RaceEventViewModel>())
                    .ToDictionary(e => e.eventID, e => e.name);

                return View(categories ??
                    new List<CategoryViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Cannot load categories from the API.";

                ViewBag.EventNames =
                    new Dictionary<int, string>();

                return View(new List<CategoryViewModel>());
            }
        }

        private async Task LoadEvents()
        {
            var events = await _apiService
                .GetAsync<List<RaceEventViewModel>>(
                    "api/Events");

            ViewBag.Events = new SelectList(
                events ?? new List<RaceEventViewModel>(),
                "eventID",
                "name");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                await LoadEvents();
            }
            catch (HttpRequestException)
            {
                ViewBag.Events = new SelectList(
                    new List<RaceEventViewModel>(),
                    "eventID", "name");

                ViewBag.ErrorMessage =
                    "Cannot load events.";
            }

            return View(new CategoryInputViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CategoryInputViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (ModelState.IsValid)
            {
                try
                {
                    using var response = await _apiService
                        .PostAsync("api/Categories", model);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] =
                            "Category created successfully!";

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
            }

            try
            {
                await LoadEvents();
            }
            catch (HttpRequestException)
            {
                ViewBag.Events = new SelectList(
                    new List<RaceEventViewModel>(),
                    "eventID", "name");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                var category = await _apiService
                    .GetAsync<CategoryViewModel>(
                        $"api/Categories/{id}");

                if (category == null)
                    return NotFound();

                var model = new CategoryInputViewModel
                {
                    EventId = category.eventID,
                    Name = category.name,
                    AgeGroup = category.agegroup,
                    Gender = category.gender,
                    DistanceKm = category.distancekm,
                    MaxParticipants = category.max_participants
                };

                ViewBag.CategoryId = id;
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Cannot load category.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, CategoryInputViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            ViewBag.CategoryId = id;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var response = await _apiService
                    .PutAsync($"api/Categories/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Category updated successfully!";

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            try
            {
                using var response = await _apiService
                    .DeleteAsync($"api/Categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Category deleted successfully!";
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
                return "Please log in as an Organiser again.";

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return "You can only change categories belonging to your own events.";

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                using var document = JsonDocument.Parse(content);

                if (document.RootElement.TryGetProperty(
                    "message", out var message))
                {
                    return message.GetString() ?? "API error.";
                }
            }
            catch (JsonException)
            {
            }

            return string.IsNullOrWhiteSpace(content)
                ? $"API error: {(int)response.StatusCode}"
                : content;
        }
    }
}
