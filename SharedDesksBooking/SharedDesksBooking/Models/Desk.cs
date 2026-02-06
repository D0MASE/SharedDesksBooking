using SharedDesksBooking.Models.Enums;

namespace SharedDesksBooking.Models
{
    public class Desk
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DeskStatus Status { get; set; } = DeskStatus.Available;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
