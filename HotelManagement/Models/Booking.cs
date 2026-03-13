using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Booking
    {
        [Key]
        public int booking_id { get; set; }
        public int user_id { get; set; }
        public int room_id { get; set; }
        public DateTime check_in { get; set; }
        public DateTime check_out { get; set; }
        public string status { get; set; }
    }
}