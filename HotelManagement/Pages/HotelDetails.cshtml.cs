using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages
{
    public class HotelDetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public HotelDetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Room Room { get; set; }
        public List<string> FacilitiesList { get; set; }

        public void OnGet(int id)
        {
            Room = _context.Rooms.FirstOrDefault(r => r.room_id == id);

            if (Room != null && Room.facilities != null)
            {
                FacilitiesList = Room.facilities
                .Split(',')
                .Select(f => f.Trim())
                .ToList();
            }
            else
            {
                FacilitiesList = new List<string>();
            }
        }
    }
}