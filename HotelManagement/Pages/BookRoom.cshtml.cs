using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;

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
            Room = _context.Rooms.FirstOrDefault(r => r.room_id == id);

            if (Room == null)
                return RedirectToPage("/Hotels");

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

            if (CheckIn >= CheckOut)
            {
                ModelState.AddModelError("", "Check-out must be after check-in");
                return Page();
            }

            // ?? TÍNH TI?N
            int totalDays = (CheckOut - CheckIn).Days;
            decimal totalPrice = Room.price * totalDays;

            // ?? L?U BOOKING LUÔN
            var booking = new Booking
            {
                user_id = userId.Value,
                room_id = id,
                check_in = CheckIn,
                check_out = CheckOut,
                status = "unpaid" // ch?a thanh toán
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // ?? dùng booking_id làm orderId
            string orderId = booking.booking_id + "_" + DateTime.Now.Ticks;

            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

            string requestId = Guid.NewGuid().ToString();

            string amount = ((int)totalPrice).ToString();
            string orderInfo = "Booking Room " + Room.title;

            string extraData = booking.booking_id.ToString();

            // ? QUAN TR?NG: redirect v? PaymentSuccess
            string redirectUrl = $"https://localhost:7096/PaymentSuccess";

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