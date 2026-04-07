using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class ManageUsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageUsersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; }
        public Dictionary<int, decimal> TotalSpent { get; set; }
        public string Search { get; set; }

        public void OnGet(string search)
        {
            Search = search;

            var query = _context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Payment)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.name.Contains(search) ||
                    u.email.Contains(search) || 
                    u.phone.Contains(search));
            }

            Users = query.ToList();

            TotalSpent = Users.ToDictionary(
                u => u.user_id,
                u => (u.Bookings ?? new List<Booking>())
                    .Where(b => b.status != "cancelled" && b.Payment != null)
                    .Sum(b => b.final_price ?? 0)
            );
        }

        public IActionResult OnPostToggle(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null) return RedirectToPage();

            user.is_active = !user.is_active;

            _context.SaveChanges();

            var userId = HttpContext.Session.GetInt32("UserId");

            LogHelper.Log(
                _context,
                HttpContext,
                userId,
                "TOGGLE_USER",
                $"User ID {user.user_id} turned {(user.is_active ? "ON" : "OFF")}"
            );

            return RedirectToPage();
        }
    }
}
