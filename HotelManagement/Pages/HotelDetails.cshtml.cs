using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Pages
{
    public class HotelDetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public HotelDetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Room Room { get; set; }
        public List<string> FacilitiesList { get; set; }

        public List<Review> Reviews { get; set; }
        [BindProperty]
        public double Rating { get; set; }
        [BindProperty]
        public string Comment { get; set; }

        [BindProperty]
        public int reviewId { get; set; }

        [BindProperty]
        public string replyText { get; set; }

        public double AvgStar { get; set; }

        [BindProperty]
        public int? parentReplyId { get; set; }

        public List<RoomImage> RoomImages { get; set; }

        public IActionResult OnPostReply(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Login");

            var reply = new ReviewReply
            {
                review_id = reviewId,
                parent_reply_id = parentReplyId, // ?? quan tr?ng
                user_id = userId.Value,
                comment = replyText,
                created_at = DateTime.Now
            };

            //M?i thêm vào ?? làm Manage Review
            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user.banned_until != null && user.banned_until > DateTime.Now)
            {
                TempData["BanMessage"] =
                    $"You are banned until {user.banned_until:dd/MM/yyyy}. Reason: {user.ban_reason}";

                return RedirectToPage(new { id });
            }

            _context.ReviewReplies.Add(reply);
            _context.SaveChanges();

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostReview(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToPage("/Login");

            var review = new Review
            {
                user_id = userId.Value,
                room_id = id,
                rating = Rating,
                comment = Comment,
                created_at = DateTime.Now
            };

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user.banned_until != null && user.banned_until > DateTime.Now)
            {
                    TempData["BanMessage"] =
                        $"You are banned until {user.banned_until:dd/MM/yyyy}. Reason: {user.ban_reason}";

                return RedirectToPage(new { id });
            }


            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToPage(new { id });
        }

        public void OnGet(int id, int? star)
        {
            RoomImages = _context.RoomImages
    .Where(i => i.room_id == id)
    .ToList();

            Room = _context.Rooms
                .FirstOrDefault(r => r.room_id == id && r.is_active == true);

            if (Room != null && Room.facilities != null)
            {
                FacilitiesList = Room.facilities
                    .Split(',')
                    .Select(f => f.Trim())
                    .ToList();
            }
            else
            {
                FacilitiesList = new List<string>();
            }

            var query = _context.Reviews
                .Where(r => r.room_id == id && r.is_deleted != true)
                .Include(r => r.User)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.User)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.Reactions)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.ChildReplies)
                        .ThenInclude(c => c.User)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.ChildReplies)
                        .ThenInclude(c => c.Reactions)
                .Include(r => r.Reactions)
                .AsQueryable();

            // ? FILTER
            if (star.HasValue)
            {
                double min = star.Value;
                double max = star.Value + 0.5;

                query = query.Where(r => r.rating >= min && r.rating <= max);
            }

            AvgStar = _context.Reviews
                .Where(r => r.room_id == id && r.is_deleted != true)
                .Any()
                ? _context.Reviews
                    .Where(r => r.room_id == id && r.is_deleted != true)
                    .Average(r => r.rating)
                : 0;

            Reviews = query
            .OrderByDescending(r => r.likes)
            .ThenByDescending(r => r.created_at)
            .ToList();

            foreach (var review in Reviews)
            {
                review.Replies = review.Replies?
                    .Where(r => r.is_deleted != true)
                    .ToList();

                foreach (var rep in review.Replies)
                {
                    rep.ChildReplies = rep.ChildReplies?
                        .Where(c => c.is_deleted != true)
                        .ToList();
                }
            }
        }

        public IActionResult OnPostLike(int id, int reviewId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return new JsonResult(new { notLoggedIn = true });

            var reaction = _context.ReviewReactions
                .FirstOrDefault(x => x.review_id == reviewId && x.user_id == userId);

            var review = _context.Reviews.Find(reviewId);

            if (reaction == null)
            {
                // ch?a like
                _context.ReviewReactions.Add(new ReviewReaction
                {
                    review_id = reviewId,
                    user_id = userId.Value,
                    is_like = true
                });

                review.likes++;
            }
            else if (reaction.is_like == true)
            {
                // b?m l?i ? b? like
                _context.ReviewReactions.Remove(reaction);
                review.likes--;
            }
            else
            {
                reaction.is_like = true;
                review.likes++;
                review.dislikes--;
            }

            _context.SaveChanges();
            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostDislike(int id, int reviewId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) 
                return new JsonResult(new { notLoggedIn = true });

            var reaction = _context.ReviewReactions
                .FirstOrDefault(x => x.review_id == reviewId && x.user_id == userId);

            var review = _context.Reviews.Find(reviewId);

            if (reaction == null)
            {
                _context.ReviewReactions.Add(new ReviewReaction
                {
                    review_id = reviewId,
                    user_id = userId.Value,
                    is_like = false
                });

                review.dislikes++;
            }
            else if (reaction.is_like == false)
            {
                _context.ReviewReactions.Remove(reaction);
                review.dislikes--;
            }
            else
            {
                reaction.is_like = false;
                review.dislikes++;
                review.likes--;
            }

            _context.SaveChanges();
            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostLikeReply(int id, int replyId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return new JsonResult(new { notLoggedIn = true });

            var reaction = _context.ReplyReactions
                .FirstOrDefault(x => x.reply_id == replyId && x.user_id == userId);

            var reply = _context.ReviewReplies.Find(replyId);

            bool? userReaction = null;

            if (reaction == null)
            {
                _context.ReplyReactions.Add(new ReplyReaction
                {
                    reply_id = replyId,
                    user_id = userId.Value,
                    is_like = true
                });

                reply.likes++;
                userReaction = true;
            }
            else if (reaction.is_like == true)
            {
                _context.ReplyReactions.Remove(reaction);
                reply.likes--;
                userReaction = null;
            }
            else
            {
                reaction.is_like = true;
                reply.likes++;
                reply.dislikes--;
                userReaction = true;
            }

            _context.SaveChanges();

            return new JsonResult(new
            {
                likes = reply.likes,
                dislikes = reply.dislikes,
                userReaction
            });
        }

        public IActionResult OnPostDislikeReply(int id, int replyId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return new JsonResult(new { notLoggedIn = true });

            var reaction = _context.ReplyReactions
                .FirstOrDefault(x => x.reply_id == replyId && x.user_id == userId);

            var reply = _context.ReviewReplies.Find(replyId);

            bool? userReaction = null;

            if (reaction == null)
            {
                _context.ReplyReactions.Add(new ReplyReaction
                {
                    reply_id = replyId,
                    user_id = userId.Value,
                    is_like = false // ? ?ÚNG
                });

                reply.dislikes++;
                userReaction = false;
            }
            else if (reaction.is_like == false)
            {
                _context.ReplyReactions.Remove(reaction);
                reply.dislikes--;
                userReaction = null;
            }
            else
            {
                reaction.is_like = false;
                reply.dislikes++;
                reply.likes--;
                userReaction = false;
            }

            _context.SaveChanges();

            return new JsonResult(new
            {
                likes = reply.likes,
                dislikes = reply.dislikes,
                userReaction
            });
        }


        public IActionResult OnGetFilter(int id, int? star)
        {
            var query = _context.Reviews
                .Where(r => r.room_id == id && r.is_deleted != true)
                .Include(r => r.User)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.User)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.Reactions)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.ChildReplies)
                        .ThenInclude(c => c.User)
                .Include(r => r.Replies)
                    .ThenInclude(rep => rep.ChildReplies)
                        .ThenInclude(c => c.Reactions)
                .Include(r => r.Reactions)
                .AsQueryable();

            if (star.HasValue)
            {
                query = query.Where(r => Math.Floor(r.rating) == star.Value);
            }

            var reviews = query
                .OrderByDescending(r => r.likes)
                .ThenByDescending(r => r.created_at)
                .ToList();

            foreach (var review in reviews)
            {
                review.Replies = review.Replies?
                    .Where(r => r.is_deleted != true)
                    .ToList();

                foreach (var rep in review.Replies)
                {
                    rep.ChildReplies = rep.ChildReplies?
                        .Where(c => c.is_deleted != true)
                        .ToList();
                }
            }

            return Partial("_ReviewListPartial", reviews);
        }
    }
}


