using HotelManagement.Data;
using HotelManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly AppDbContext _context;

        public LogoutModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            // ? LOG LOGOUT
            LogHelper.Log(_context, HttpContext, userId, "LOGOUT", "User logged out");

            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}