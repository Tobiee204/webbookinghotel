using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Pages.Admin
{
    public class SecurityLogsModel : PageModel
    {
        private readonly AppDbContext _context;

        public SecurityLogsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<SecurityLog> Logs { get; set; } = new();

        public string Search { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public void OnGet(string search, DateTime? fromDate, DateTime? toDate)
        {
            // ? gi? l?i giá tr? sau khi filter
            Search = search;
            FromDate = fromDate;
            ToDate = toDate;

            var query = _context.SecurityLogs
                .Include(l => l.User)
                .AsQueryable();

            // ?? SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l =>
                    (l.action != null && l.action.Contains(search)) ||
                    (l.description != null && l.description.Contains(search)) ||
                    (l.User != null && l.User.name != null && l.User.name.Contains(search))
                );
            }

            // ?? FROM DATE
            if (fromDate.HasValue)
            {
                query = query.Where(l => l.created_at >= fromDate.Value);
            }

            // ?? TO DATE (fix m?t d? li?u trong ngày)
            if (toDate.HasValue)
            {
                var to = toDate.Value.AddDays(1);
                query = query.Where(l => l.created_at < to);
            }

            // ?? DATA
            Logs = query
                .OrderByDescending(l => l.created_at)
                .Take(200)
                .ToList();
        }
    }
}