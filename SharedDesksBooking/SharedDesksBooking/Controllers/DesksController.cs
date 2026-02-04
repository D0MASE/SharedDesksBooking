using Microsoft.AspNetCore.Mvc;
using SharedDesksBooking.Services;

namespace SharedDesksBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesksController : ControllerBase
    {
        private readonly IDeskService _deskService;

        // Naudojame Dependency Injection per konstruktorių
        public DesksController(IDeskService deskService)
        {
            _deskService = deskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDesks([FromQuery] DateTime? date)
        {
            // Jei data nepateikta, naudojame šiandienos datą
            var targetDate = date ?? DateTime.Today;

            // Visa logika perduodama servisui
            var result = await _deskService.GetDesksWithAvailabilityAsync(targetDate);

            return Ok(result);
        }
    }
}