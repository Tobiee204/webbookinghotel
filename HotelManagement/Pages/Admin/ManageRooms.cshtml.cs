using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages.Admin
{
    public class ManageRoomsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageRoomsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }

        public void OnGet()
        {
            Rooms = _context.Rooms.ToList();
        }

        // DELETE
        public IActionResult OnPostDelete(int id)
        {
            var room = _context.Rooms.Find(id);

            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}