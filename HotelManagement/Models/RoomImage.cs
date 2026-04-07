using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class RoomImage
    {
        [Key]
        public int image_id { get; set; }

        public int room_id { get; set; }

        public string image_url { get; set; }

        [ForeignKey("room_id")]
        public Room Room { get; set; }
    }
}