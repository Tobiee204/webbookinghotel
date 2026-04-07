using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Helpers
{
    public static class LogHelper
    {
        public static void Log(AppDbContext context, HttpContext httpContext, int? userId, string action, string description)
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString();

            var log = new SecurityLog
            {
                user_id = userId,
                action = action,
                description = description,
                created_at = DateTime.Now,
                ip_address = ip
            };

            context.SecurityLogs.Add(log);
            context.SaveChanges();
        }
    }
}