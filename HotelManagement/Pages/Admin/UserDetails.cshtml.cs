using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages.Admin
{
    public class UserDetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserDetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public User User { get; set; }

        public void OnGet(int id)
        {
            User = _context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Room)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Payment)
                .FirstOrDefault(u => u.user_id == id);
        }
    }
}
