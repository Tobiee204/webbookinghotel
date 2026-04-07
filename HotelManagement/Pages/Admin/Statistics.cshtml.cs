using HotelManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages.Admin
{
    public class StatisticsModel : PageModel
    {
        private readonly AppDbContext _context;

        public StatisticsModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalRooms { get; set; }
        public int BookedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalUsers { get; set; }
        public int CancelledBookings { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalRevenue { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }
        public decimal VipRevenue { get; set; }
        public decimal NormalRevenue { get; set; }
        public List<decimal> MonthlyRevenue { get; set; } = new List<decimal>();
        [BindProperty(SupportsGet = true)]
        public int? Year { get; set; }

        public void OnGet()
        {
            var bookings = _context.Bookings.AsQueryable();

            if (FromDate.HasValue)
                bookings = bookings.Where(b => b.check_in >= FromDate.Value);

            if (ToDate.HasValue)
                bookings = bookings.Where(b => b.check_in <= ToDate.Value);

            TotalRooms = _context.Rooms
                .Where(r => r.is_active == true)
                .Count();
            TotalUsers = _context.Users.Count();

            BookedRooms = _context.Rooms
                .Count(r => r.status == "Booked" && r.is_active == true);

            CancelledBookings = bookings.Count(b => b.status == "Cancelled");

            AvailableRooms = _context.Rooms
                .Count(r => r.status == "Available" && r.is_active == true);

            TotalReviews = _context.Reviews.Count();

            TotalRevenue = bookings
                .Where(b => b.status == "confirmed" && b.Payment != null)
                .Sum(b => (decimal?)b.final_price) ?? 0;

            VipRevenue = bookings
                .Where(b => b.Room.room_category == "VIP"
                         && b.status == "confirmed"
                         && b.Payment != null)
                .Sum(b => (decimal?)b.final_price) ?? 0;

            NormalRevenue = bookings
                .Where(b => b.Room.room_category == "Normal"
                         && b.status == "confirmed"
                         && b.Payment != null)
                .Sum(b => (decimal?)b.final_price) ?? 0;

            int year = Year ?? DateTime.Now.Year;

            MonthlyRevenue = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var revenue = _context.Bookings
                    .Where(b => b.status == "confirmed"
                             && b.Payment != null
                             && b.check_in.Year == year
                             && b.check_in.Month == month)
                    .Sum(b => (decimal?)b.final_price) ?? 0;

                MonthlyRevenue.Add(revenue);
            }
        }
    }
}