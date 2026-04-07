using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

            return RedirectToPage("/Admin/ManageServices");
        }
    }
}