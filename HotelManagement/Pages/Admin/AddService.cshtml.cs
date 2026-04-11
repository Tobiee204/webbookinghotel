using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class AddServiceModel : PageModel
    {
        private readonly AppDbContext _context;

        public AddServiceModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Service Service { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IActionResult OnPost()
        {
            var adminId = HttpContext.Session.GetInt32("UserId");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields!";
                return Page();
            }

            if (Upload != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Upload.FileName);
                var path = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                Service.image = "/images/" + fileName;
            }

            _context.Services.Add(Service);
            _context.SaveChanges();

            LogHelper.Log(
                _context,
                HttpContext,
                adminId,
                "ADD_SERVICE",
                $"Admin added service {Service.name} (ID: {Service.service_id})"
            );

            TempData["SuccessMessage"] = "Service added successfully!";

            return RedirectToPage("/Admin/ManageServices");
        }
    }
}