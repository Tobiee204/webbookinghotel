using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;
using HotelManagement.Helpers;


namespace HotelManagement.Pages
{
    public class BookRoomModel : PageModel
    {
        private readonly AppDbContext _context;

        public BookRoomModel(AppDbContext context)
        {
            _context = context;
        }

        public Room Room { get; set; }

        [BindProperty]
        public DateTime CheckIn { get; set; }

        [BindProperty]
        public DateTime CheckOut { get; set; }

        [BindProperty]
        public int Guests { get; set; }

        [BindProperty]
        public int? SelectedOfferId { get; set; }

        public List<UserOffer> UserOffers { get; set; } = new List<UserOffer>();

        private string SignSHA256(string message, string secretKey)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);
            var data = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(key))
            {
                return BitConverter.ToString(hmac.ComputeHash(data)).Replace("-", "").ToLower();
            }
        }

        public IActionResult OnGet(int id)
        {
            Room = _context.Rooms
                .FirstOrDefault(r => r.room_id == id && r.is_active == true);

            CheckIn = DateTime.Today;
            CheckOut = DateTime.Today.AddDays(1);

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                UserOffers = _context.UserOffers
                    .Where(u => u.user_id == userId && !u.is_used)
                    .Include(u => u.Offer) // ?? FIX CHÍNH
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Login");

            Room = _context.Rooms.FirstOrDefault(r => r.room_id == id);

            if (Room == null)
                return RedirectToPage("/Hotels");

            // ? Không cho ch?n ngày quá kh?
            if (CheckIn < DateTime.Today)
            {
                ModelState.AddModelError("", "Check-in cannot be in the past");

                // ?? LOAD L?I DATA
                Room = _context.Rooms.FirstOrDefault(r => r.room_id == id);

                var userIdReload = HttpContext.Session.GetInt32("UserId");
                if (userIdReload != null)
                {
                    UserOffers = _context.UserOffers
                        .Where(u => u.user_id == userIdReload && !u.is_used)
                        .Include(u => u.Offer)
                        .ToList();
                }

                return Page();
            }

            // ? Check-out ph?i l?n h?n check-in
            if (CheckOut <= CheckIn)
            {
                ModelState.AddModelError("", "Check-out must be after check-in");

                // ?? LOAD L?I DATA
                Room = _context.Rooms.FirstOrDefault(r => r.room_id == id);

                var userIdReload = HttpContext.Session.GetInt32("UserId");
                if (userIdReload != null)
                {
                    UserOffers = _context.UserOffers
                        .Where(u => u.user_id == userIdReload && !u.is_used)
                        .Include(u => u.Offer)
                        .ToList();
                }

                return Page();
            }

            // ?? TÍNH TI?N
            int totalDays = (CheckOut - CheckIn).Days;
            decimal totalPrice = Room.final_price * totalDays;

            // ?? APPLY VOUCHER (CH?N)
            decimal finalPrice = totalPrice;

            if (SelectedOfferId.HasValue)
            {
                var offer = _context.Offers
                    .FirstOrDefault(o => o.offer_id == SelectedOfferId.Value);

                if (offer != null)
                {
                    bool valid = true;
                    var now = DateTime.Now;

                    // CONDITION
                    if (offer.type == "condition")
                    {
                        if (offer.min_amount.HasValue && totalPrice < offer.min_amount.Value)
                            valid = false;
                    }

                    // EVENT
                    if (offer.type == "event")
                    {
                        if (offer.start_date > now || offer.end_date < now)
                            valid = false;
                    }

                    if (valid)
                    {
                        if (offer.discount_type == "percent")
                        {
                            finalPrice -= totalPrice * offer.discount_value / 100;
                        }
                        else
                        {
                            finalPrice -= offer.discount_value;
                        }

                        // ?? MARK USED
                        var userOffer = _context.UserOffers.FirstOrDefault(x =>
                            x.offer_id == SelectedOfferId.Value &&
                            x.user_id == userId &&
                            !x.is_used);

                        if (userOffer != null)
                        {
                            userOffer.is_used = true;
                        }
                    }
                }
            }

            // ?? FIX âm ti?n
            if (finalPrice < 0) finalPrice = 0;

            // ?? L?u BOOKING LUÔN
            var booking = new Booking
            {
                user_id = userId.Value,
                room_id = id,
                check_in = CheckIn,
                check_out = CheckOut,
                status = "unpaid",
                total_price = totalPrice,
                final_price = finalPrice
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // ? LOG BOOKING
            LogHelper.Log(_context, HttpContext, userId, "BOOKING", $"Booked room ID {id}");

            // ?? dùng booking_id làm orderId
            string orderId = booking.booking_id + "_" + DateTime.Now.Ticks;

            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

            string requestId = Guid.NewGuid().ToString();

            string amount = ((int)finalPrice).ToString();
            string orderInfo = "Booking Room " + Room.title;

            string extraData = booking.booking_id.ToString();

            //redirect v? PaymentSuccess
            string redirectUrl = $"https://localhost:7096/PaymentSuccess?id={id}";

            string ipnUrl = "https://example.com/ipn";

            string rawHash =
                $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=captureWallet";

            string signature = SignSHA256(rawHash, secretKey);

            var requestData = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl = ipnUrl,
                extraData = extraData,
                requestType = "captureWallet",
                lang = "vi",
                signature
            };

            var client = new HttpClient();
            var response = await client.PostAsync(endpoint,
                new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("payUrl", out var payUrlElement))
            {
                return Content("MoMo Error: " + json);
            }

            string payUrl = payUrlElement.GetString();

            return Redirect(payUrl);
        }
    }
}