using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Helpers;

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

        [BindProperty]
        public List<IFormFile> Uploads { get; set; }

        [BindProperty]
        public IFormFile MainImage { get; set; }

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var room = _context.Rooms.Find(Room.room_id);

            if (room == null)
            {
                return RedirectToPage("/Admin/ManageRooms");
            }

            // UPDATE TEXT
            room.title = Room.title;
            room.room_type = Room.room_type;
            room.price = Room.price;
            room.guests = Room.guests;
            room.bed_type = Room.bed_type;
            room.description = Room.description;
            room.facilities = Room.facilities;
            room.room_category = Room.room_category;
            room.status = Room.status;

            // ?? UPDATE MAIN IMAGE
            if (MainImage != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImage.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    MainImage.CopyTo(stream);
                }

                room.image = "/images/" + fileName;
            }

            // ?? ADD MORE IMAGES (FIX CHU?N)
            if (Uploads != null && Uploads.Count > 0)
            {
                foreach (var file in Uploads)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        _context.RoomImages.Add(new RoomImage
                        {
                            room_id = room.room_id, // ?? dùng room.room_id cho ch?c
                            image_url = "/images/" + fileName
                        });
                    }
                }
            }

            // ?? SAVE 1 L?N CU?I
            _context.SaveChanges();

            // LOG EDIT ROOM
            var adminId = HttpContext.Session.GetInt32("UserId");

            LogHelper.Log(
                _context,
                HttpContext,
                adminId,
                "EDIT_ROOM",
                $"Admin updated room {room.room_id} | Title: {room.title} | Price: {room.price} | Status: {room.status}"
            );

            TempData["SuccessMessage"] = "Update room successfully!";
            return RedirectToPage("/Admin/ManageRooms");
        }
    }
}