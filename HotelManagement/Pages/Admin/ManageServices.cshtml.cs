using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

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
            var adminId = HttpContext.Session.GetInt32("UserId");

            var s = _context.Services.Find(id);

            if (s != null)
            {
                string serviceName = s.name;

                _context.Services.Remove(s);
                _context.SaveChanges();

                LogHelper.Log(
                    _context,
                    HttpContext,
                    adminId,
                    "DELETE_SERVICE",
                    $"Admin deleted service {id} | Name: {serviceName}"
                );

                TempData["SuccessMessage"] = "Service deleted successfully!";
            }

            return RedirectToPage();
        }
    }
}