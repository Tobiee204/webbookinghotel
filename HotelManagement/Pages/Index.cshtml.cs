using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            // ? TÍNH RATING
            AvgRating = _context.Reviews
                .Where(r => r.is_deleted != true)
                .GroupBy(r => r.room_id)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(x => x.rating)
                );

            // ?? L?Y TOP 3 HOT ROOM
            HotRooms = AvgRating
                .OrderByDescending(x => x.Value)
                .Take(3)
                .Select(x => x.Key)
                .ToList();

            // ?? CH? L?Y 3 ROOM HOT
            PopularRooms = _context.Rooms
                .Where(r => HotRooms.Contains(r.room_id)
                            && r.is_active == true)
                .ToList();

            // (optional) gi? ?úng th? t? HOT
            PopularRooms = PopularRooms
                .OrderByDescending(r => AvgRating[r.room_id])
                .ToList();

            // ? SERVICES (l?y 4 cái)
            Services = _context.Services
                .Take(4)
                .ToList();
        }

        public JsonResult OnGetSuggest(string keyword)
        {
            var rooms = _context.Rooms
                .Where(r => r.title.Contains(keyword))
                .Select(r => new { r.room_id, r.title })
                .Take(5)
                .ToList();

            return new JsonResult(rooms);
        }
    }
}