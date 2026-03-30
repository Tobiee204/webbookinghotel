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

        public void OnGet()
        {
            var bookings = _context.Bookings.AsQueryable();

            if (FromDate.HasValue)
                bookings = bookings.Where(b => b.check_in >= FromDate.Value);

            if (ToDate.HasValue)
                bookings = bookings.Where(b => b.check_in <= ToDate.Value);

            TotalRooms = _context.Rooms.Count();
            TotalUsers = _context.Users.Count();

            BookedRooms = bookings.Count(b => b.status == "Booked");
            CancelledBookings = bookings.Count(b => b.status == "Cancelled");

            AvailableRooms = TotalRooms - BookedRooms;

            TotalReviews = _context.Reviews.Count();

            TotalRevenue = _context.Payments
                .Where(p => !FromDate.HasValue || p.payment_date >= FromDate)
                .Where(p => !ToDate.HasValue || p.payment_date <= ToDate)
                .Sum(p => (decimal?)p.amount) ?? 0;
        }
    }
}