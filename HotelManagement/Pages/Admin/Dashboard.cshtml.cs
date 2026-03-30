using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalRooms { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<Booking> RecentBookings { get; set; }

        public void OnGet()
        {
            TotalRooms = _context.Rooms.Count();
            TotalUsers = _context.Users.Count();
            TotalBookings = _context.Bookings.Count();

            TotalRevenue = _context.Payments.Sum(p => (decimal?)p.amount) ?? 0;

            RecentBookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Include(b => b.Payment)
                .OrderByDescending(b => b.booking_id)
                .Take(5)
                .ToList();
        }
    }
}