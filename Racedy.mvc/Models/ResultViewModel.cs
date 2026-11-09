
namespace RaceDay.MVC.Models
{
    public class ResultViewModel
    {
        public int resultID { get; set; }
        public int registrationID { get; set; }
        public int participantID { get; set; }
        public int eventID { get; set; }
        public int categoryID { get; set; }
        public TimeSpan finish_time { get; set; }
        public int position { get; set; }
        public string split_times { get; set; } = "";
        public DateTime created_at { get; set; }
    }
}
