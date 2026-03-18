using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

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
        public IFormFile Upload { get; set; }

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

            if (Upload != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Upload.FileName);
                string path = Path.Combine(_env.WebRootPath, "images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                Room.image = "/images/" + fileName;
            }
            else
            {
                Room.image = "/images/default.jpg";
            }

            _context.Rooms.Add(Room);
            _context.SaveChanges();

            SuccessMessage = "? Room added successfully!";

            return RedirectToPage("/Admin/AddRoom");
        }

    }
}