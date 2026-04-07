using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class SecurityLog
    {
        [Key]
        public int log_id { get; set; }

        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public User? User { get; set; }

        public string action { get; set; }

        public string description { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;

        public string? ip_address { get; set; }
    }
}