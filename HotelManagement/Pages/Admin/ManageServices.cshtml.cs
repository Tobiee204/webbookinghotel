using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages.Admin
{
    public class ManageServicesModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageServicesModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Service> Services { get; set; }

        public void OnGet()
        {
            Services = _context.Services.ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            var s = _context.Services.Find(id);

            if (s != null)
            {
                _context.Services.Remove(s);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Service deleted successfully!";
            }

            return RedirectToPage();
        }
    }
}