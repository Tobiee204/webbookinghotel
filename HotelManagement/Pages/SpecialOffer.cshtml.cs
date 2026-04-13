using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Pages
{
    public class SpecialOfferModel : PageModel
    {
        private readonly AppDbContext _context;

        public SpecialOfferModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Offer> Offers { get; set; } = new List<Offer>();
        public List<UserOffer> UserOffers { get; set; } = new List<UserOffer>();

        public int? userId => HttpContext.Session.GetInt32("UserId");

        public void OnGet()
        {
            Offers = _context.Offers
                .Where(o => o.is_active && !o.is_delete)
                .ToList();

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                UserOffers = _context.UserOffers
                    .Where(u => u.user_id == userId)
                    .ToList();
            }
        }

        public bool IsValid(Offer o)
        {
            if (o.type == "event")
            {
                var now = DateTime.Now;
                return now >= o.start_date && now <= o.end_date;
            }
            return true;
        }

        public IActionResult OnPost(int offerId)
        {
            if (userId == null)
                return RedirectToPage("/Login");

            var exist = _context.UserOffers
                .FirstOrDefault(x => x.offer_id == offerId && x.user_id == userId);

            if (exist == null)
            {
                _context.UserOffers.Add(new UserOffer
                {
                    offer_id = offerId,
                    user_id = userId.Value
                });

                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}