using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Review
    {
        [Key]
        public int review_id { get; set; }
        public int user_id { get; set; }
        public int hotel_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public DateTime created_at { get; set; }
    }
}