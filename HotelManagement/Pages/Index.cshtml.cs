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
        public Dictionary<int, double> AvgRating { get; set; }
        public List<int> HotRooms { get; set; }
        public List<Service> Services { get; set; }

        public void OnGet()
        {
            // ? CH? l?y room active
            PopularRooms = _context.Rooms
                .Where(r => r.status == "Available" && r.is_active == true)
                .Take(3)
                .ToList();

            // ? TÍNH RATING
            AvgRating = _context.Reviews
                .Where(r => r.is_deleted != true)
                .GroupBy(r => r.room_id)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(x => x.rating)
                );

            // ?? HOT ROOM (top rating)
            HotRooms = AvgRating
                .OrderByDescending(x => x.Value)
                .Take(3)
                .Select(x => x.Key)
                .ToList();

            // ? SERVICES (l?y 4 cái)
            Services = _context.Services
                .Take(4)
                .ToList();
        }
    }
}