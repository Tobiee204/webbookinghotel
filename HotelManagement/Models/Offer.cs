using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Offer
    {
        [Key]
        public int offer_id { get; set; }

        public string code { get; set; }

        public decimal discount_value { get; set; }

        public string discount_type { get; set; } // percent | amount

        public decimal? min_amount { get; set; }

        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }

        public bool is_active { get; set; }

        public string type { get; set; } // condition | event
    }
}