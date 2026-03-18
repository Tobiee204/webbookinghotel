using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Room
    {
        [Key]
        public int room_id { get; set; }

        [Required(ErrorMessage = "Room Title is required")]
        public string title { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string room_type { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 1000000000, ErrorMessage = "Price must be greater than 0")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "Guests is required")]
        [Range(1, 10, ErrorMessage = "Guests must be at least 1")]
        public int guests { get; set; }

        [Required(ErrorMessage = "Bed Type is required")]
        public string bed_type { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }

        [Required(ErrorMessage = "Facilities is required")]
        public string facilities { get; set; }

        public string status { get; set; }

        public string? image { get; set; }
    }
}