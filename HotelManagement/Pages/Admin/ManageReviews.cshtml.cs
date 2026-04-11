using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Helpers;

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
            var adminId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.banned_until = banUntil;
                user.ban_reason = banReason;
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"User {user.name} has been banned successfully.";

                LogHelper.Log(
                    _context,
                    HttpContext,
                    adminId,
                    "BAN_USER",
                    $"Admin banned user {userId} until {banUntil} | Reason: {banReason}"
                    );
            }

            else
            {
                TempData["ErrorMessage"] = "User not found!";
            }

            return RedirectToPage(new { roomId = Request.Query["roomId"] });
        }

        public IActionResult OnPostUnban(int userId)
        {
            var adminId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.banned_until = null;
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"User {user.name} has been unbanned.";

                LogHelper.Log(
                    _context,
                    HttpContext,
                    adminId,
                    "UNBAN_USER",
                    $"Admin unbanned user {userId}"
                );
            }

            return RedirectToPage(new { roomId = Request.Query["roomId"] });
        }

        public IActionResult OnPostDelete(int id)
        {
            var adminId = HttpContext.Session.GetInt32("UserId");

            var r = _context.Reviews.Find(id);

            if (r != null)
            {
                r.is_deleted = true; // soft delete
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Review deleted successfully.";

                LogHelper.Log(
                    _context,
                    HttpContext,
                    adminId,
                    "DELETE_REVIEW",
                    $"Admin deleted review {id} (UserId: {r.user_id}, RoomId: {r.room_id})"
                );
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteReply(int id)
        {
            var adminId = HttpContext.Session.GetInt32("UserId");

            var reply = _context.ReviewReplies.Find(id);

            if (reply != null)
            {
                reply.is_deleted = true;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Reply deleted successfully.";

                LogHelper.Log(
                    _context,
                    HttpContext,
                    adminId,
                    "DELETE_REPLY",
                    $"Admin deleted reply {id} (ReviewId: {reply.review_id}, UserId: {reply.user_id})"
                );
            }

            return RedirectToPage(new { roomId = Request.Query["roomId"] });
        }
    }
}