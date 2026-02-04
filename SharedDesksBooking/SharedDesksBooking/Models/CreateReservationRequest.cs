using System.ComponentModel.DataAnnotations;

public class CreateReservationRequest
{
    public int DeskId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}