using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@@$!%*?&]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string password { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be exactly 10 digits")]
        public string phone { get; set; }

        public string? avatar { get; set; }

        public DateTime? created_at { get; set; }

        public List<Booking> Bookings { get; set; }
    }
}