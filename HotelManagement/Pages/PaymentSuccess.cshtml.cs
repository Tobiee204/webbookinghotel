using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Helpers;

namespace HotelManagement.Pages
{
    public class PaymentSuccessModel : PageModel
    {
        private readonly AppDbContext _context;

        public PaymentSuccessModel(AppDbContext context)
        {
            _context = context;
        }

        public bool IsSuccess { get; set; }

        public IActionResult OnGet()
        {
            var roomId = Request.Query["id"];
            var resultCode = Request.Query["resultCode"];
            var bookingIdStr = Request.Query["extraData"];

            //FAIL
            if (resultCode != "0" || string.IsNullOrEmpty(bookingIdStr))
            {
                return RedirectToPage("/BookRoom", new { id = roomId });
            }

            //SUCCESS
            IsSuccess = true;

            int bookingId = int.Parse(bookingIdStr);

            var booking = _context.Bookings.FirstOrDefault(b => b.booking_id == bookingId);

            if (booking != null)
            {
                booking.status = "pending";

                var room = _context.Rooms.FirstOrDefault(r => r.room_id == booking.room_id);
                if (room != null)
                {
                    room.status = "pending";
                }

                //THÊM PAYMENT (QUAN TR?NG)
                var payment = new Payment
                {
                    booking_id = bookingId,
                    amount = (booking.check_out - booking.check_in).Days * room.price,
                    payment_method = "MoMo",
                    payment_date = DateTime.Now
                };

                _context.Payments.Add(payment);

                _context.SaveChanges();

                // ? LOG PAYMENT
                var userId = HttpContext.Session.GetInt32("UserId");
                LogHelper.Log(_context, HttpContext, userId, "PAYMENT", $"Payment success for booking ID {bookingId}");
            }

            return Page();
        }
    }
}