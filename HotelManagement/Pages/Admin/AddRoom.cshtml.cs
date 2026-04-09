using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class AddRoomModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AddRoomModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Room Room { get; set; }

        [BindProperty]
        public IFormFile MainImage { get; set; }

        [BindProperty]
        public List<IFormFile> Uploads { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {

            Console.WriteLine("?? OnPost CALLED");
            foreach (var error in ModelState)
            {
                foreach (var subError in error.Value.Errors)
                {
                    Console.WriteLine($"? {error.Key}: {subError.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "? Please fix the errors below!";
                return Page();
            }

            Room.title ??= "";
            Room.description ??= "";
            Room.facilities ??= "";
            Room.bed_type ??= "";
            Room.status ??= "Available";

            if (Room.guests <= 0)
                Room.guests = 1;

            if (Room.price <= 0)
            {
                ErrorMessage = "? Price must be greater than 0!";
                return Page();
            }

            // SAVE ROOM FIRST
            _context.Rooms.Add(Room);
            _context.SaveChanges();

            // ?? MAIN IMAGE ? l?u vào Room.image
            if (MainImage != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImage.FileName);
                string path = Path.Combine(_env.WebRootPath, "images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    MainImage.CopyTo(stream);
                }

                Room.image = "/images/" + fileName;
            }

            // ?? SUB IMAGES
            if (Uploads != null && Uploads.Count > 0)
            {
                foreach (var file in Uploads)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(_env.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _context.RoomImages.Add(new RoomImage
                    {
                        room_id = Room.room_id,
                        image_url = "/images/" + fileName,
                    });
                }
            }

            _context.SaveChanges();

            // ? LOG ADD ROOM
            var adminId = HttpContext.Session.GetInt32("UserId");
            LogHelper.Log(_context, HttpContext, adminId, "ADD_ROOM", $"Added room: {Room.title}");

            SuccessMessage = "? Room added successfully!";

            TempData["SuccessMessage"] = "Add room successfully!";
            return Page();
        }

    }
}