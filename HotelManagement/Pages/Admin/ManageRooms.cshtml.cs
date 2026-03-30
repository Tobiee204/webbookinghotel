using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

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
            Rooms = _context.Rooms.ToList();
        }

        // DELETE
        public IActionResult OnPostDelete(int id)
        {
            var room = _context.Rooms.Find(id);

            if (room == null)
                return RedirectToPage();

            // ? KHÔNG CHO XÓA n?u phòng ?ang pending ho?c booked
            if (room.status == "pending" || room.status == "booked")
            {
                TempData["Error"] = "Room is currently processing or already booked. Cannot delete!";
                return RedirectToPage();
            }

            // ? KHÔNG CHO XÓA n?u ?ã t?ng có booking
            var hasBooking = _context.Bookings.Any(b => b.room_id == id);
            if (hasBooking)
            {
                TempData["Error"] = "This room has booking history. Cannot delete!";
                return RedirectToPage();
            }

            _context.Rooms.Remove(room);
            _context.SaveChanges();

            TempData["Success"] = "Room deleted successfully!";
            return RedirectToPage();
        }
    }
}