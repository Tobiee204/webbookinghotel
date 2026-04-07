using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;
using System.Linq;
using HotelManagement.Helpers;

namespace HotelManagement.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private static string otpCode;
        private static string resetEmail;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            // ? ch? thêm n?u ch?a có l?i
            if (string.IsNullOrWhiteSpace(Email) && !ModelState.ContainsKey("Email"))
            {
                ModelState.AddModelError("Email", "Email is required");
            }

            if (string.IsNullOrWhiteSpace(Password) && !ModelState.ContainsKey("Password"))
            {
                ModelState.AddModelError("Password", "Password is required");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _context.Users
                .FirstOrDefault(u => u.email.ToLower() == Email.ToLower());

            if (user != null && !user.is_active)
            {
                ModelState.AddModelError("Email", "Account has been disabled!");
                return Page();
            }

            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.password, Password);

            if (result == PasswordVerificationResult.Failed)
            {
                // LOG FAIL LOGIN
                LogHelper.Log(_context, HttpContext, user.user_id, "LOGIN_FAIL", "Wrong password");

                ModelState.AddModelError("Password", "Wrong password");
                return Page();
            }

            HttpContext.Session.SetString("UserEmail", user.email);
            HttpContext.Session.SetString("UserName", user.name);
            HttpContext.Session.SetInt32("UserId", user.user_id);
            HttpContext.Session.SetString("Avatar", user.avatar ?? "");
            HttpContext.Session.SetString("Role", user.role);

            LogHelper.Log(_context, HttpContext, user.user_id, "LOGIN", "User logged in");

            if (user.role == "admin")
            {
                return RedirectToPage("/Admin/Dashboard"); // ? admin vào ?ây
            }
            else
            {
                return RedirectToPage("/Index"); // user th??ng
            }
        }

        // ===== SEND OTP =====
        public class EmailRequest
        {
            public string email { get; set; } // ?? CH? TH??NG
        }

        public IActionResult OnPostSendOTP()
        {
            var email = Request.Form["email"].ToString();

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email null");

            email = email.Trim().ToLower();

            var user = _context.Users
                .FirstOrDefault(u => u.email.ToLower() == email);

            if (user == null)
                return new JsonResult("no_email");

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("ResetEmail", email);

            SendOTP(email, otp);

            return new JsonResult("sent");
        }

        // ===== VERIFY OTP =====
        public IActionResult OnPostVerifyOTP()
        {
            var otp = Request.Form["otp"].ToString().Trim();
            var savedOtp = HttpContext.Session.GetString("OTP");

            Console.WriteLine("========== VERIFY DEBUG ==========");
            Console.WriteLine("INPUT OTP: [" + otp + "]");
            Console.WriteLine("SAVED OTP: [" + savedOtp + "]");
            Console.WriteLine("=================================");

            if (!string.IsNullOrEmpty(otp) && otp == savedOtp)
                return Content("ok");

            return new JsonResult("fail");
        }

        // ===== RESET PASSWORD =====
        public IActionResult OnPostResetPassword(string newPass)
        {
            var email = HttpContext.Session.GetString("ResetEmail");

            var user = _context.Users
                .FirstOrDefault(u => u.email == email);

            if (user == null)
                return BadRequest();

            var hasher = new PasswordHasher<User>();
            user.password = hasher.HashPassword(user, newPass);

            _context.SaveChanges();

            // ? LOG RESET PASSWORD
            LogHelper.Log(_context, HttpContext, user.user_id, "RESET_PASSWORD", "User reset password via OTP");

            return new JsonResult("done");
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
                Subject = "Reset Password OTP",
                Body = $"Your OTP code is: {otp}"
            })
            {
                smtp.Send(message);
            }
        }
    }
}