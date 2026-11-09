
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // Everyone can view events.
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Event
                .AsNoTracking()
                .OrderBy(e => e.event_date)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/Events/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var raceEvent = await _context.Event.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(raceEvent);
        }

        // POST: api/Events
        // Only Organisers can create events.
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] EventInput input)
        {
            var organiserId = GetOrganiserId();

            if (organiserId == null)
            {
                return Unauthorized(new
                {
                    message = "Log in as an Organiser first."
                });
            }

            if (!IsValidEvent(input))
            {
                return BadRequest(new
                {
                    message = "Provide valid event details. Event type must be Run, Walk or Cycle."
                });
            }

            int newId = (await _context.Event
                .MaxAsync(e => (int?)e.eventID) ?? 0) + 1;

            var raceEvent = new RaceEvent
            {
                eventID = newId,
                organiserID = organiserId.Value,
                name = input.Name.Trim(),
                description = input.Description.Trim(),
                event_date = input.EventDate,
                location = input.Location.Trim(),
                Distance_km = input.DistanceKm,
                event_type = input.EventType.Trim(),
                status = "Upcoming",
                created_at = DateTime.Now
            };

            _context.Event.Add(raceEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = raceEvent.eventID },
                raceEvent);
        }

        // PUT: api/Events/1
        // Organisers can update only their own events.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventInput input)
        {
            var organiserId = GetOrganiserId();

            if (organiserId == null)
            {
                return Unauthorized(new
                {
                    message = "Organiser login required."
                });
            }

            var raceEvent = await _context.Event.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (raceEvent.organiserID != organiserId.Value)
            {
                return StatusCode(403, new
                {
                    message = "You do not have permission to modify this event."
                });
            }

            if (!IsValidEvent(input))
            {
                return BadRequest(new
                {
                    message = "Check the event details and distance."
                });
            }

            raceEvent.name = input.Name.Trim();
            raceEvent.description = input.Description.Trim();
            raceEvent.event_date = input.EventDate;
            raceEvent.location = input.Location.Trim();
            raceEvent.Distance_km = input.DistanceKm;
            raceEvent.event_type = input.EventType.Trim();

            await _context.SaveChangesAsync();

            return Ok(raceEvent);
        }

        // DELETE: api/Events/1
        // Organisers can delete only their own events.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var organiserId = GetOrganiserId();

            if (organiserId == null)
            {
                return Unauthorized(new
                {
                    message = "Organiser login required."
                });
            }

            var raceEvent = await _context.Event.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (raceEvent.organiserID != organiserId.Value)
            {
                return StatusCode(403, new
                {
                    message = "You do not have permission to delete this event."
                });
            }

            bool hasCategories = await _context.Category
                .AnyAsync(c => c.eventID == id);

            bool hasRegistrations = await _context.Registration
                .AnyAsync(r => r.eventID == id);

            if (hasCategories || hasRegistrations)
            {
                return Conflict(new
                {
                    message = "This event has categories or registrations and cannot be deleted."
                });
            }

            _context.Event.Remove(raceEvent);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event deleted successfully."
            });
        }

        // Get logged-in Organiser ID from session.
        private int? GetOrganiserId()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return null;
            }

            return HttpContext.Session.GetInt32("UserId");
        }

        // Validate event information.
        private static bool IsValidEvent(EventInput input)
        {
            return input != null
                && !string.IsNullOrWhiteSpace(input.Name)
                && input.Name.Length <= 50
                && !string.IsNullOrWhiteSpace(input.Description)
                && input.Description.Length <= 100
                && !string.IsNullOrWhiteSpace(input.Location)
                && input.Location.Length <= 50
                && input.EventDate != default
                && input.DistanceKm > 0
                && new[] { "Run", "Walk", "Cycle" }.Contains(
                    input.EventType?.Trim(),
                    StringComparer.OrdinalIgnoreCase);
        }
    }

    public class EventInput
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public decimal DistanceKm { get; set; }
        public string EventType { get; set; } = "";
    }
}
