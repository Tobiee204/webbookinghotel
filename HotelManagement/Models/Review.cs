using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Review
    {
        [Key]
        public int review_id { get; set; }
        public int user_id { get; set; }
        public int room_id { get; set; }

        public double rating { get; set; }

        [MaxLength(500)]
        public string comment { get; set; }

        public DateTime created_at { get; set; }

        public bool is_deleted { get; set; }

        public int likes { get; set; }
        public int dislikes { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }

        [ForeignKey("room_id")]
        public Room Room { get; set; }
        public List<ReviewReply> Replies { get; set; }
        public List<ReviewReaction> Reactions { get; set; }
    }
}