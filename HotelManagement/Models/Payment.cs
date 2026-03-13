using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Models
{
    public class Payment
    {
        [Key]
        public int payment_id { get; set; }
        public int booking_id { get; set; }
        public decimal amount { get; set; }
        public string payment_method { get; set; }
        public DateTime payment_date { get; set; }
    }
}