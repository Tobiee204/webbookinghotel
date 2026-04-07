using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

namespace HotelManagement.Pages.Admin
{
    public class EditOfferModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditOfferModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Offer Offer { get; set; }

        // ?? LOAD DATA
        public IActionResult OnGet(int id)
        {
            Offer = _context.Offers.FirstOrDefault(o => o.offer_id == id);

            if (Offer == null)
            {
                return RedirectToPage("/Admin/ManageOffers");
            }

            return Page();
        }

        // ?? UPDATE
        public IActionResult OnPost()
        {
            var offerInDb = _context.Offers.FirstOrDefault(o => o.offer_id == Offer.offer_id);

            if (offerInDb == null)
            {
                return RedirectToPage("/Admin/ManageOffers");
            }

            // VALIDATE
            if (string.IsNullOrEmpty(Offer.code))
            {
                ModelState.AddModelError("", "Code required");
                return Page();
            }

            if (Offer.discount_value <= 0)
            {
                ModelState.AddModelError("", "Invalid discount");
                return Page();
            }

            // CONDITION
            if (Offer.type == "condition")
            {
                if (!Offer.min_amount.HasValue)
                {
                    ModelState.AddModelError("", "Min amount required");
                    return Page();
                }

                Offer.start_date = null;
                Offer.end_date = null;
            }

            // EVENT
            if (Offer.type == "event")
            {
                if (!Offer.start_date.HasValue || !Offer.end_date.HasValue)
                {
                    ModelState.AddModelError("", "Date required");
                    return Page();
                }

                if (Offer.start_date > Offer.end_date)
                {
                    ModelState.AddModelError("", "Invalid date");
                    return Page();
                }

                Offer.min_amount = null;
            }

            // ?? UPDATE FIELD
            offerInDb.code = Offer.code;
            offerInDb.discount_value = Offer.discount_value;
            offerInDb.discount_type = Offer.discount_type;
            offerInDb.type = Offer.type;
            offerInDb.min_amount = Offer.min_amount;
            offerInDb.start_date = Offer.start_date;
            offerInDb.end_date = Offer.end_date;
            offerInDb.is_active = Offer.is_active;

            _context.SaveChanges();

            var userId = HttpContext.Session.GetInt32("UserId");

            LogHelper.Log(
                _context,
                HttpContext,
                userId,
                "EDIT_OFFER",
                $"Updated offer '{offerInDb.code}' (ID: {offerInDb.offer_id})"
            );

            return RedirectToPage("/Admin/ManageOffers");
        }
    }
}