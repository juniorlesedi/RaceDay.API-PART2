
namespace RaceDay.MVC.Models
{
    public class RegistrationViewModel
    {
        public int registrationID { get; set; }
        public int participantID { get; set; }
        public int eventID { get; set; }
        public int categoryID { get; set; }
        public DateTime registrationdate { get; set; }
        public string status { get; set; } = "";
        public string paymentstatus { get; set; } = "";
        public int entryfee { get; set; }
    }
}
