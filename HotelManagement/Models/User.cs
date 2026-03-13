using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public DateTime? created_at { get; set; }
    }
}