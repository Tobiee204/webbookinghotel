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
        public Dictionary<int, double> AvgRating { get; set; }
        public List<int> HotRooms { get; set; }

        public void OnGet(string status, string room)
        {
            var query = _context.Rooms
            .Where(r => r.is_active == true)
            .AsQueryable();

            // FILTER STATUS
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(r => r.status.ToLower() == status.ToLower());
            }

            // SEARCH
            if (!string.IsNullOrEmpty(room))
            {
                room = room.ToLower();

                query = query.Where(r =>
                    r.title.ToLower().Contains(room) ||
                    r.room_type.ToLower().Contains(room));
            }

            AvgRating = _context.Reviews
                    .Where(r => r.is_deleted != true)
                    .GroupBy(r => r.room_id)
                    .ToDictionary(
                    g => g.Key,
                    g => g.Average(x => x.rating)
    );

            HotRooms = AvgRating
                .OrderByDescending(x => x.Value)
                .Take(3)
                .Select(x => x.Key)
                .ToList();

            Rooms = query.ToList();
        }
    }
}