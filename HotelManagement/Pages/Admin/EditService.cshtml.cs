using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class EditServiceModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditServiceModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Service Service { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public void OnGet(int id)
        {
            Service = _context.Services.Find(id);
        }

        public IActionResult OnPost()
        {
            var adminId = HttpContext.Session.GetInt32("UserId");

            var existing = _context.Services.Find(Service.service_id);

            if (existing == null) return NotFound();

            existing.name = Service.name;
            existing.description = Service.description;


            if (Upload != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Upload.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                existing.image = "/images/" + fileName;
            }

            _context.SaveChanges();

            LogHelper.Log(
                _context,
                HttpContext,
                adminId,
                "EDIT_SERVICE",
                $"Admin updated service {existing.service_id} | Name: {existing.name}"
            );

            return RedirectToPage("ManageServices");
        }
    }
}