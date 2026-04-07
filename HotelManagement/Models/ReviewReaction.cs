using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class ReviewReaction
    {
        public int id { get; set; }

        public int review_id { get; set; }
        [ForeignKey("review_id")]
        public Review Review { get; set; }

        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User User { get; set; }

        public bool is_like { get; set; }
    }
}
