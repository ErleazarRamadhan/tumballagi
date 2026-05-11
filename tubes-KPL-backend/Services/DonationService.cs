using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;

namespace tubes_KPL_backend.Services
{
    public class DonationService
    {
        private readonly AppDbContext _context;
        public DonationService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Donation> GetDonationByIdAsync(int id)
        {
            var donations = await _context.Donations.FirstOrDefaultAsync(u => u.Id == id);
            return donations;
        }
        public async Task<List<Donation>> GetAllDonationsAsync()
        {
            var donations = await _context.Donations.ToListAsync();
            return donations;
        }
    }
}
