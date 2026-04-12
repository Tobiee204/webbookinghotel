using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public void OnGet(string status, string room, string category, DateTime? checkIn, DateTime? checkOut, int? guests, string bedType)
        {
            var query = _context.Rooms
            .Where(r => r.is_active == true)
            .AsQueryable();

            // FILTER STATUS
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(r => r.status.ToLower() == status.ToLower());
            }

            // FILTER CATEGORY
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(r => r.room_category.ToLower() == category.ToLower());
            }

            // SEARCH
            if (!string.IsNullOrEmpty(room))
            {
                room = room.ToLower();

                query = query.Where(r =>
                    r.title.ToLower().Contains(room) ||
                    r.room_type.ToLower().Contains(room));
            }

            // GUESTS
            if (guests.HasValue)
            {
                query = query.Where(r => r.guests >= guests.Value);
            }

            // BED TYPE
            if (!string.IsNullOrEmpty(bedType))
            {
                query = query.Where(r => r.bed_type == bedType);
            }

            // DATE AVAILABLE CHECK
            if (checkIn.HasValue && checkOut.HasValue)
            {
                query = query.Where(r => !_context.Bookings.Any(b =>
                    b.room_id == r.room_id &&
                    b.status == "confirmed" &&
                    (checkIn < b.check_out && checkOut > b.check_in)
                ));
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

            Rooms = query
                .ToList()
                .OrderByDescending(r => HotRooms.Contains(r.room_id))
                .ThenByDescending(r => r.room_id)
                .ToList();

            if (Rooms.Count == 0)
            {
                TempData["NotFound"] = "No suitable rooms found!";
            }
        }

        public JsonResult OnGetSuggest(string keyword)
        {
            var rooms = _context.Rooms
                .Where(r => r.is_active == true && r.title.Contains(keyword))
                .Select(r => new { r.room_id, r.title })
                .Take(5)
                .ToList();

            return new JsonResult(rooms);
        }
    }
}