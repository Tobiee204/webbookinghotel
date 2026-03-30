using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Pages.Admin
{
    public class ManageBookingsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageBookingsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Booking> Bookings { get; set; }

        public void OnGet(string status)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(b => b.status == status);
            }

            Bookings = query.ToList();
        }

        public IActionResult OnPostUpdateStatus(int id, string status)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.booking_id == id);

            if (booking != null)
            {
                booking.status = status;

                var room = _context.Rooms.FirstOrDefault(r => r.room_id == booking.room_id);

                if (room != null)
                {
                    if (status == "confirmed")
                        room.status = "booked";

                    if (status == "cancelled")
                        room.status = "available";
                }

                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}