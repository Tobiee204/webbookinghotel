using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Pages.Admin
{
    public class ManageReviewsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageReviewsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Review> Reviews { get; set; }
        public List<Room> Rooms { get; set; }

        public int? RoomId { get; set; }

        public void OnGet(int? roomId)
        {
            Rooms = _context.Rooms.ToList();

            if (roomId != null)
            {
                RoomId = roomId;

                Reviews = _context.Reviews
                    .Where(r => r.room_id == roomId && r.is_deleted != true)
                    .Include(r => r.User)
                    .Include(r => r.Replies.Where(rep => rep.is_deleted != true))
                        .ThenInclude(x => x.User)
                    .ToList();
            }
        }

        public IActionResult OnPostBan(int userId, DateTime banUntil, string banReason)
        {
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.banned_until = banUntil;
                user.ban_reason = banReason;
                _context.SaveChanges();
            }

            return RedirectToPage(new { roomId = Request.Query["roomId"] });
        }

        public IActionResult OnPostUnban(int userId)
        {
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.banned_until = null;
                _context.SaveChanges();
            }

            return RedirectToPage(new { roomId = Request.Query["roomId"] });
        }

        public IActionResult OnPostDelete(int id)
        {
            var r = _context.Reviews.Find(id);

            if (r != null)
            {
                r.is_deleted = true; // soft delete
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteReply(int id)
        {
            var reply = _context.ReviewReplies.Find(id);

            if (reply != null)
            {
                reply.is_deleted = true; // ? soft delete
                _context.SaveChanges();
            }

            return RedirectToPage(new { roomId = Request.Query["roomId"] });
        }
    }
}