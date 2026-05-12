using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs.Donation;
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
        public async Task<Donation?> GetDonationByIdAsync(int id)
        {
            var donations = await _context.Donations.FirstOrDefaultAsync(u => u.Id == id);
            return donations;
        }
        public async Task<List<Donation>> GetAllDonationsAsync()
        {
            var donations = await _context.Donations.ToListAsync();
            return donations;
        }

        public async Task<CreateDonationResponseDTO> CreateDonationAsync(CreateDonationRequestDTO request)
        {
            // Validasi nilai donasi wajib > 0 agar tidak ada transaksi nominal nol/negatif.
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Nominal donasi harus lebih dari 0.");
            }

            // Pastikan user (donatur) valid.
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                throw new KeyNotFoundException("User tidak ditemukan.");
            }

            // Ambil campaign tujuan yang akan ditambahkan total dananya.
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == request.CampaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign tidak ditemukan.");
            }

            // Atomic transaction: catat donasi + update total campaign harus sukses bersama.
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var donation = new Donation
                {
                    UserId = request.UserId,
                    CampaignId = request.CampaignId,
                    Amount = request.Amount,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Donations.Add(donation);

                // Update akumulasi dana campaign secara sistematis setelah donasi valid.
                campaign.CollectedAmount += request.Amount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CreateDonationResponseDTO
                {
                    DonationId = donation.Id,
                    CampaignId = campaign.Id,
                    DonationAmount = donation.Amount,
                    UpdatedCampaignTotal = campaign.CollectedAmount,
                    CreatedDate = donation.CreatedDate
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteDonationAsync(int id)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(d => d.Id == id);
            if (donation == null)
            {
                return false;
            }

            var campaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == donation.CampaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign tidak ditemukan untuk donasi ini.");
            }

            // Atomic transaction: hapus donasi + koreksi total campaign harus sinkron.
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Donations.Remove(donation);
                campaign.CollectedAmount -= donation.Amount;
                if (campaign.CollectedAmount < 0)
                {
                    campaign.CollectedAmount = 0;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
