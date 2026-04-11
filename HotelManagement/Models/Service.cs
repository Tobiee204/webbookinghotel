using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Service
    {
        [Key]
        public int service_id { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        public string name { get; set; }
        public string? image { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }
    }
}

