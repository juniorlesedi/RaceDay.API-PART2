
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class ResultInputViewModel
    {
        [Range(1, int.MaxValue)]
        public int RegistrationId { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int Position { get; set; }

        public string SplitTimes { get; set; } = "";
    }
}
