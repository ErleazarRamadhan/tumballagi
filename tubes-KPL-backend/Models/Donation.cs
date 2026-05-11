using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace tubes_KPL_backend.Models
{
    [PrimaryKey(nameof(Id))] 
    public class Donation
    {
        
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
