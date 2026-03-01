using Microsoft.AspNetCore.Mvc;
using SharedDesksBooking.Services;

namespace SharedDesksBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DesksController(IDeskService deskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDesks([FromQuery] DateTime? date)
    {
        // Jei data nepateikta, naudojame šiandienos datą
        var targetDate = date ?? DateTime.Today;

        // Visa logika perduodama servisui
        var result = await deskService.GetDesksWithAvailabilityAsync(targetDate);

        return Ok(result);
    }
}