using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Hotel
    {
        [Key]
        public int hotel_id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public double rating { get; set; }
    }
}
