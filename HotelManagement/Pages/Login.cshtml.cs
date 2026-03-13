using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using System.Linq;

namespace HotelManagement.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users
                .FirstOrDefault(u => u.email == Email && u.password == Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login");
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}