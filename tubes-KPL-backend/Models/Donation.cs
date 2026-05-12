using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace tubes_KPL_backend.Models
{
    [PrimaryKey(nameof(Id))] 
    public class Donation
    {

        [PrimaryKey(nameof(Id))]
        public class Donation
        {
            public int Id { get; set; }

            // User (donatur) yang melakukan donasi.
            public int UserId { get; set; }

            // Campaign tujuan dari donasi.
            public int CampaignId { get; set; }

            // Timestamp saat donasi tercatat di sistem.
            public DateTime CreatedDate { get; set; }

            // Nominal uang yang didonasikan.
            public decimal Amount { get; set; }

            [ForeignKey(nameof(UserId))]
            public User? User { get; set; }

            // Relasi ke campaign agar histori donasi bisa ditelusuri per campaign.
            [ForeignKey(nameof(CampaignId))]
            public Campaign? Campaign { get; set; }
        }
}
