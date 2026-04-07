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

        public IActionResult OnPostUpdateStatus(int id, string status, string reason, string customReason)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.booking_id == id);

            if (booking != null)
            {
                booking.status = status;

                // ?? l?u lý do
                if (status == "cancelled")
                {
                    booking.cancel_reason = reason == "Lý do khác" ? customReason : reason;
                }

                var room = _context.Rooms.FirstOrDefault(r => r.room_id == booking.room_id);

                if (room != null)
                {
                    if (status == "confirmed")
                        room.status = "booked";

                    if (status == "cancelled")
                        room.status = "Available";
                }

                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}