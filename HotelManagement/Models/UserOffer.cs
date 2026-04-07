using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    [Table("user_offers")]
    public class UserOffer
    {
        public int id { get; set; }

        [ForeignKey("User")]
        public int user_id { get; set; }

        [ForeignKey("Offer")]
        public int offer_id { get; set; }

        public bool is_used { get; set; }

        public User User { get; set; }
        public Offer Offer { get; set; }
    }
}
