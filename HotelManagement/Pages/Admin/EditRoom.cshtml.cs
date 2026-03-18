using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages.Admin
{
    public class EditRoomModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditRoomModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; }

        // LOAD DATA KHI M? TRANG EDIT
        public IActionResult OnGet(int id)
        {
            Room = _context.Rooms.Find(id);

            if (Room == null)
            {
                return RedirectToPage("/Admin/ManageRooms");
            }

            return Page();
        }

        // UPDATE DATA KHI B?M NÚT
        public IActionResult OnPost()
        {
            // check validate
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var room = _context.Rooms.Find(Room.room_id);

            if (room == null)
            {
                return RedirectToPage("/Admin/ManageRooms");
            }

            // UPDATE T?T C? FIELD (GI?NG ADD ROOM)
            room.title = Room.title;
            room.room_type = Room.room_type;
            room.price = Room.price;
            room.guests = Room.guests;
            room.bed_type = Room.bed_type;
            room.description = Room.description;
            room.facilities = Room.facilities;
            room.status = Room.status;

            _context.SaveChanges();

            // thông báo thành công
            TempData["SuccessMessage"] = "Update room successfully!";

            return RedirectToPage("/Admin/ManageRooms");
        }
    }
}