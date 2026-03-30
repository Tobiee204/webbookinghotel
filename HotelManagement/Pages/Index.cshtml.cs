using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Room> PopularRooms { get; set; }

        public void OnGet()
        {
            // L?y 3 phòng ??u tiên (ho?c b?n có th? order theo giá / rating)
            PopularRooms = _context.Rooms
                .Where(r => r.status == "Available")
                .Take(3)
                .ToList();
        }
    }
}