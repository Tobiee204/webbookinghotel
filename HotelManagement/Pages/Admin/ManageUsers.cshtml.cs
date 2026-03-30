using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public void OnGet()
        {
            Users = _context.Users
        .Include(u => u.Bookings)
            .ThenInclude(b => b.Payment)
        .ToList();

            TotalSpent = Users.ToDictionary(
                u => u.user_id,
                u => u.Bookings
                      .Where(b => b.Payment != null)
                      .Sum(b => b.Payment.amount)
                    );
        }

        public IActionResult OnPostDelete(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
