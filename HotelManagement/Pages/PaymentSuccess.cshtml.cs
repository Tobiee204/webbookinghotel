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

        public IActionResult OnGet()
        {
            var resultCode = Request.Query["resultCode"];
            var bookingIdStr = Request.Query["extraData"];

            // N?u thanh toán th?t b?i ? không làm gì
            if (resultCode != "0" || string.IsNullOrEmpty(bookingIdStr))
                return Page();

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

                // ?? THÊM PAYMENT (QUAN TR?NG)
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