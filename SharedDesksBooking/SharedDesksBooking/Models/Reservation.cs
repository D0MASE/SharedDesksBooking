using Microsoft.AspNetCore.Routing.Constraints;

namespace SharedDesksBooking.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int DeskId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
