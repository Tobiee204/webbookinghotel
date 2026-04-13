using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class ManageRoomsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageRoomsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }

        public void OnGet()
        {
            // AUTO UPDATE STATUS
            var today = DateTime.Today;

            var rooms = _context.Rooms.ToList();

            foreach (var room in rooms)
            {
                var activeBooking = _context.Bookings
                         .Where(b => b.room_id == room.room_id
                                  && b.check_out > today
                                  && b.status != "cancelled")
                         .OrderByDescending(b => b.booking_id)
                         .FirstOrDefault();

                if (activeBooking != null)
                {
                    if (activeBooking.status == "pending")
                    {
                        room.status = "Pending";
                    }
                    else if (activeBooking.status == "confirmed")
                    {
                        room.status = "Booked";
                    }
                }
            }

            Rooms = rooms;
        }

        public IActionResult OnPostToggle(int id)
        {
            var room = _context.Rooms.Find(id);

            if (room == null) return RedirectToPage();

            room.is_active = !room.is_active;

            _context.SaveChanges();

            var userId = HttpContext.Session.GetInt32("UserId");

            LogHelper.Log(
                _context,
                HttpContext,
                userId,
                "TOGGLE_ROOM",
                $"Room ID {room.room_id} turned {(room.is_active ? "ON" : "OFF")}"
            );

            return RedirectToPage();
        }
    }
}
