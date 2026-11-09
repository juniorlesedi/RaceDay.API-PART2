
namespace RaceDay.MVC.Models
{
    public class CategoryViewModel
    {
        public int categoryID { get; set; }
        public int eventID { get; set; }

        public string name { get; set; } = "";
        public string agegroup { get; set; } = "";
        public string gender { get; set; } = "";

        public decimal distancekm { get; set; }
        public int max_participants { get; set; }
    }
}
