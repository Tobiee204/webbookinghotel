using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class ReplyReaction
    {
        public int id { get; set; }

        public int reply_id { get; set; }
        [ForeignKey("reply_id")]
        public ReviewReply Reply { get; set; }
        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User User { get; set; }

        public bool is_like { get; set; }
    }
}
