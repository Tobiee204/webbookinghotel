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

        public void OnGet(string status, string room)
        {
            var query = _context.Rooms.AsQueryable();

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

            Rooms = query.ToList();
        }
    }
}