using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class AddOfferModel : PageModel
    {
        private readonly AppDbContext _context;

        public AddOfferModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Offer Offer { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            // VALIDATE
            if (string.IsNullOrEmpty(Offer.code))
            {
                ModelState.AddModelError("", "Code is required");
                return Page();
            }

            if (Offer.discount_value <= 0)
            {
                ModelState.AddModelError("", "Discount must be > 0");
                return Page();
            }

            // ?? CONDITION
            if (Offer.type == "condition")
            {
                if (!Offer.min_amount.HasValue)
                {
                    ModelState.AddModelError("", "Min amount required");
                    return Page();
                }

                // reset event fields
                Offer.start_date = null;
                Offer.end_date = null;
            }

            // ?? EVENT
            if (Offer.type == "event")
            {
                if (!Offer.start_date.HasValue || !Offer.end_date.HasValue)
                {
                    ModelState.AddModelError("", "Start/End date required");
                    return Page();
                }

                if (Offer.start_date > Offer.end_date)
                {
                    ModelState.AddModelError("", "Invalid date range");
                    return Page();
                }

                // reset condition field
                Offer.min_amount = null;
            }

            // ?? DEFAULT
            Offer.is_active = true;

            _context.Offers.Add(Offer);
            _context.SaveChanges();

            var userId = HttpContext.Session.GetInt32("UserId");

            LogHelper.Log(
                _context,
                HttpContext,
                userId,
                "ADD_OFFER",
                $"Created offer code '{Offer.code}' with {Offer.discount_value} {Offer.discount_type}"
            );

            return RedirectToPage("/Admin/ManageOffers");
        }
    }
}