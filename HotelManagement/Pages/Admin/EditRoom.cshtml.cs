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
        public IFormFile? MainImage { get; set; }

        public List<RoomImage> RoomImages { get; set; }

        // LOAD DATA KHI M? TRANG EDIT
        public IActionResult OnGet(int id)
        {
            Room = _context.Rooms.Find(id);

            if (Room == null)
            {
                return RedirectToPage("/Admin/ManageRooms");
            }

            // LOAD SUB IMAGES
            RoomImages = _context.RoomImages
                            .Where(x => x.room_id == id)
                            .ToList();

            return Page();
        }

        // UPDATE DATA KHI B?M NÚT
        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {
                RoomImages = _context.RoomImages
                .Where(x => x.room_id == Room.room_id)
                .ToList();

                TempData["ErrorMessage"] = "Please check your input!";
                return Page();
            }

            var room = _context.Rooms.Find(Room.room_id);

            if (room == null)
            {
                return RedirectToPage("/Admin/ManageRooms");
            }

            if (MainImage == null)
            {
                Room.image = room.image;
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

            // UPDATE MAIN IMAGE
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

            // ADD MORE IMAGES
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
                            room_id = room.room_id,
                            image_url = "/images/" + fileName
                        });
                    }
                }
            }

            _context.SaveChanges();

            var adminId = HttpContext.Session.GetInt32("UserId");

            LogHelper.Log(
                _context,
                HttpContext,
                adminId,
                "EDIT_ROOM",
                $"Admin updated room {room.room_id} | Title: {room.title} | Price: {room.price} | Status: {room.status}"
            );

            TempData["SuccessMessage"] = " Update room successfully!";
            return RedirectToPage("/Admin/ManageRooms");
        }

        public IActionResult OnPostDeleteImage(int id)
        {
            var image = _context.RoomImages.Find(id);

            if (image != null)
            {
                // XÓA FILE TRONG WWWROOT
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    image.image_url.TrimStart('/')
                );

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // XÓA DB
                _context.RoomImages.Remove(image);
                _context.SaveChanges();
            }

            return RedirectToPage(new { id = image?.room_id });
        }
    }
}