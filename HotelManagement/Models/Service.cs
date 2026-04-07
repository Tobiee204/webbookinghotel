using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Service
    {
        [Key]
        public int service_id { get; set; }
            public string name { get; set; }
            public string? image { get; set; }
            public string description { get; set; }
    }
}

