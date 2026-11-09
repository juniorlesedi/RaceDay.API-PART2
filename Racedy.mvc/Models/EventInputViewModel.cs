
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class EventInputViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(50)]
        public string Location { get; set; } = "";

        [Range(0.01, 999.99)]
        public decimal DistanceKm { get; set; }

        [Required]
        [RegularExpression("Run|Walk|Cycle",
            ErrorMessage = "Choose Run, Walk or Cycle.")]
        public string EventType { get; set; } = "";
    }
}
