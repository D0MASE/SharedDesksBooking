using System.ComponentModel.DataAnnotations;

public class CreateReservationRequest
{
    [Required(ErrorMessage = "Stalas yra privalomas")]
    public int DeskId { get; set; }

    [Required(ErrorMessage = "Vardas yra privalomas")]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pavardė yra privaloma")]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}