using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Room
    {
        [Key]
        public int room_id { get; set; }
        public int hotel_id { get; set; }
        public string room_type { get; set; }
        public decimal price { get; set; }
        public string status { get; set; }
    }
}