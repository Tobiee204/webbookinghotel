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


        public void OnGet()
        {
            Offers = _context.Offers
                .Where(o => !o.is_delete)
                .OrderByDescending(o => o.offer_id)
                .ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            var offer = _context.Offers.FirstOrDefault(o => o.offer_id == id);

            if (offer != null)
            {
                offer.is_delete = true;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Offer deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Offer not found!";
            }

            return RedirectToPage();
        }
    }
}