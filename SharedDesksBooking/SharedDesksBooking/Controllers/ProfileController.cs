using Microsoft.AspNetCore.Mvc;
using SharedDesksBooking.Services;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserProfile([FromQuery] string firstName, [FromQuery] string lastName)
    {
        var profile = await profileService.GetUserProfileAsync(firstName, lastName);

        if (profile == null)
        {
            return BadRequest("Vardas ir pavardė yra privalomi.");
        }

        return Ok(profile);
    }
}