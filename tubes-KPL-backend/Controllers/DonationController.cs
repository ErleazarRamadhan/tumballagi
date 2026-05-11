using Microsoft.AspNetCore.Mvc;

namespace tubes_KPL_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationController : ControllerBase
    {
        private readonly DonationService _donationService;

        public DonationController(DonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Donation>> GetDonationById(int id)
        {
            var donation = await _donationService.GetDonationByIdAsync(id);
            if (donation == null)
            {
                return NotFound();
            }
            return Ok(donation);
        }

        [HttpGet]
        public async Task<ActionResult<List<Donation>>> GetAllDonations()
        {
            var donations = await _donationService.GetAllDonationsAsync();
            return Ok(donations);
        }
    }
}
