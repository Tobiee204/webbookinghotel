using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    [Table("review_replies")]
    public class ReviewReply
    {
        [Key]
        public int reply_id { get; set; }
        public int review_id { get; set; }
        public int? parent_reply_id { get; set; }
        public int user_id { get; set; }

        [MaxLength(300)]
        public string comment { get; set; }
        public int likes { get; set; } = 0;
        public int dislikes { get; set; } = 0;
        public bool? is_deleted { get; set; } = false;
        public DateTime created_at { get; set; }

        [ForeignKey("review_id")]
        public Review Review { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }
        public ReviewReply ParentReply { get; set; }
        public List<ReviewReply> ChildReplies { get; set; }
        public List<ReplyReaction> Reactions { get; set; }
    }
}
