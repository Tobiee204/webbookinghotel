using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using HotelManagement.Helpers;

namespace HotelManagement.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;
        private PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string action, string InputOTP)
        {
            // ===== VERIFY OTP =====
            if (action == "verify")
            {
                var savedOTP = TempData["OTP"]?.ToString();
                var userData = TempData["UserData"]?.ToString();

                TempData.Keep();

                if (InputOTP != savedOTP)
                {
                    TempData["Error"] = "Invalid OTP!";
                    TempData["ShowOTP"] = true;
                    TempData["ShowPopup"] = "error";
                    return Page();
                }

                var user = System.Text.Json.JsonSerializer.Deserialize<User>(userData);

                _context.Users.Add(user);
                _context.SaveChanges();

                // LOG REGISTER
                LogHelper.Log(_context, HttpContext, user.user_id, "REGISTER", "New user registered");

                TempData["Success"] = "Registration successful!";
                TempData["ShowPopup"] = "success";

                return Page();
            }


            // VALIDATE EMAIL TRÙNG
            if (_context.Users.Any(u => u.email == User.email))
            {
                TempData["Error"] = "Email already exists!";
                return Page();
            }

            // VALIDATE PASSWORD
            if (!Regex.IsMatch(User.password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$"))
            {
                TempData["Error"] = "Password must include uppercase, lowercase, number and special character!";
                return Page();
            }

            // VALIDATE PHONE
            if (!Regex.IsMatch(User.phone, @"^\d{10}$"))
            {
                TempData["Error"] = "Phone must be exactly 10 digits!";
                return Page();
            }

            // HASH PASSWORD
            User.password = _hasher.HashPassword(User, User.password);

            // SET TIME
            User.created_at = DateTime.Now;

            // TAO OTP
            var otp = new Random().Next(100000, 999999).ToString();

            TempData["OTP"] = otp;
            TempData["Email"] = User.email;
            TempData["UserData"] = System.Text.Json.JsonSerializer.Serialize(User);

            // G?i EMAIL
            SendOTP(User.email, otp);

            TempData["Success"] = "OTP has been sent to your email!";

            TempData["ShowOTP"] = true;
            return Page();
        }

        private void SendOTP(string email, string otp)
        {
            var fromAddress = new MailAddress("hotelangel295@gmail.com");
            var toAddress = new MailAddress(email);

            const string fromPassword = "swdq otiu qdtr pghg";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "OTP Verification",
                Body = $"Your OTP code is: {otp}"
            })
            {
                smtp.Send(message);
            }
        }

        public IActionResult OnPostResendOTP()
        {
            var email = TempData["Email"]?.ToString();

            if (email != null)
            {
                var otp = new Random().Next(100000, 999999).ToString();

                TempData["OTP"] = otp;
                TempData["Email"] = email;
                TempData.Keep();

                SendOTP(email, otp);
            }

            return new JsonResult("sent");
        }
    }
}