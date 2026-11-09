
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class RaceEventViewModel
    {
        public int eventID { get; set; }

        public int organiserID { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(50)]
        public string name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        public string description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        public DateTime event_date { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(50)]
        public string location { get; set; } = string.Empty;

        [Range(0.01, 999.99,
            ErrorMessage = "Distance must be greater than zero")]
        public decimal Distance_km { get; set; }

        [Required(ErrorMessage = "Event type is required")]
        [StringLength(50)]
        public string event_type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        public string status { get; set; } = string.Empty;

        public DateTime created_at { get; set; }
    }
}
