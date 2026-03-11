using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
    }
}