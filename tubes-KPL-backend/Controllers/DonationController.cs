using Microsoft.AspNetCore.Mvc;
using tubes_KPL_backend.DTOs.Donation;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;

namespace tubes_KPL_backend.Controllers;

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

    [HttpPost]
    public async Task<ActionResult<CreateDonationResponseDTO>> CreateDonation(CreateDonationRequestDTO request)
    {
        try
        {
            // Endpoint utama transaksi donasi: mencatat donasi dan update total campaign.
            var result = await _donationService.CreateDonationAsync(request);
            return CreatedAtAction(nameof(GetDonationById), new { id = result.DonationId }, result);
        }
        catch (ArgumentException ex)
        {
            // Error validasi input dari client (contoh: nominal <= 0).
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            // Error referensi data tidak valid (user/campaign tidak ditemukan).
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonation(int id)
    {
        try
        {
            var deleted = await _donationService.DeleteDonationAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Donation tidak ditemukan." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
