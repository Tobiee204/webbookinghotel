using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;


namespace HotelManagement.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProfileModel(AppDbContext context)
        {
            _context = context;
        }

        public User CurrentUser { get; set; }
        public List<BookingViewModel> Bookings { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                Response.Redirect("/Login");
                return;
            }

            CurrentUser = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (CurrentUser == null)
            {
                Response.Redirect("/Login");
                return;
            }

            Bookings = _context.Bookings
                .Where(b => b.user_id == userId)
                .Join(_context.Rooms,
                      b => b.room_id,
                      r => r.room_id,
                      (b, r) => new BookingViewModel
                      {
                          RoomTitle = r.title,
                          CheckIn = b.check_in,
                          CheckOut = b.check_out,
                          Status = b.status
                      }).ToList();
        }

        public IActionResult OnPostUpdateName(string name)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user == null)
                return RedirectToPage("/Login");

            user.name = name;

            _context.SaveChanges();

            HttpContext.Session.SetString("UserName", name);

            return RedirectToPage();
        }

        public IActionResult OnPostUpdatePassword(string password)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user == null)
                return RedirectToPage("/Login");

            var hasher = new PasswordHasher<User>();
            user.password = hasher.HashPassword(user, password);

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostSendOTP(string oldPass, string newPass)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user == null) return Content("User not found");

            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.password, oldPass);

            if (result == PasswordVerificationResult.Failed)
                return Content("Wrong old password");

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("OTP_PROFILE", otp);
            HttpContext.Session.SetString("NEW_PASS", newPass);

            SendOTP(user.email, otp);

            return Content("ok");
        }

        public IActionResult OnPostVerifyOTP(string otp)
        {
            var savedOtp = HttpContext.Session.GetString("OTP_PROFILE");

            if (otp != savedOtp)
                return Content("fail");

            var userId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            var newPass = HttpContext.Session.GetString("NEW_PASS");

            var hasher = new PasswordHasher<User>();
            user.password = hasher.HashPassword(user, newPass);

            _context.SaveChanges();

            return Content("done");
        }
        private void SendOTP(string email, string otp)
        {
            var from = new MailAddress("hotelangel295@gmail.com");
            var to = new MailAddress(email);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(from.Address, "swdq otiu qdtr pghg")
            };

            var msg = new MailMessage(from, to)
            {
                Subject = "OTP Change Password",
                Body = $"Your OTP: {otp}"
            };

            smtp.Send(msg);
        }

        public async Task<IActionResult> OnPostUploadAvatar(IFormFile avatarFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user == null)
                return RedirectToPage("/Login");

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);

                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                user.avatar = "/images/" + fileName;

                _context.SaveChanges();
            }

            HttpContext.Session.SetString("Avatar", user.avatar);

            return RedirectToPage();
        }
    }
}
