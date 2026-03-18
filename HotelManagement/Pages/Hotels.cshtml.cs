using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages
{
    public class HotelsModel : PageModel
    {
        private readonly AppDbContext _context;

        public HotelsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }

        public void OnGet()
        {
            Rooms = _context.Rooms
    .Where(r => r.status == "Available")
    .ToList();
        }
    }
}