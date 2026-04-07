using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages
{
    public class ServicesModel : PageModel
    {
        private readonly AppDbContext _context;

        public ServicesModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Service> Services { get; set; }

        public void OnGet()
        {
            Services = _context.Services
                .ToList();
        }
    }
}