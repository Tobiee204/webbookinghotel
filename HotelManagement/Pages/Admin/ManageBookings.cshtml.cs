using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Helpers;


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
            var adminId = HttpContext.Session.GetInt32("UserId");

            var booking = _context.Bookings.FirstOrDefault(b => b.booking_id == id);

            if (booking != null)
            {
                string oldStatus = booking.status;

                booking.status = status;

                string finalReason = "";

                // ?? l?u lý do
                if (status == "cancelled")
                {
                    finalReason = reason == "Lý do khác" ? customReason : reason;
                    booking.cancel_reason = finalReason;
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

                LogHelper.Log(
                    _context,
                    HttpContext,
                    adminId,
                    "UPDATE_BOOKING_STATUS",
                    $"Admin updated booking {id} | From: {oldStatus} -> {status} | RoomId: {booking.room_id}" +
                    (status == "cancelled" ? $" | Reason: {finalReason}" : "")
                );

            }

            return RedirectToPage();
        }
    }
}