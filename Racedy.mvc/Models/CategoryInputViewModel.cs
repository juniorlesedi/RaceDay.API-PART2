
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CategoryInputViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select an event.")]
        public int EventId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string AgeGroup { get; set; } = "";

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = "";

        [Range(0.01, 999.99)]
        public decimal DistanceKm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxParticipants { get; set; }
    }
}
