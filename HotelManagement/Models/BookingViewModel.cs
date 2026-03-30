namespace HotelManagement.Models
{
    public class BookingViewModel
    {
        public string RoomTitle { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; }
    }
}
