using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages.Admin
{
    public class ManageOffersModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageOffersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Offer> Offers { get; set; }

        // ?? LOAD DATA
        public void OnGet()
        {
            Offers = _context.Offers
                .OrderByDescending(o => o.offer_id)
                .ToList();
        }

        // ?? DELETE
        public IActionResult OnGetDelete(int id)
        {
            var offer = _context.Offers.FirstOrDefault(o => o.offer_id == id);

            if (offer != null)
            {
                _context.Offers.Remove(offer);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}