using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Booking
    {
        [Key]
        public int booking_id { get; set; }

        [ForeignKey("User")]
        public int user_id { get; set; }
        public User User { get; set; }

        public int room_id { get; set; }
        public Room Room { get; set; }

        public DateTime check_in { get; set; }
        public DateTime check_out { get; set; }
        public string status { get; set; }

        public Payment? Payment { get; set; }
    }
}