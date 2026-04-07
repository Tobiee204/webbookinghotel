using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewReply> ReviewReplies { get; set; }
        public DbSet<ReviewReaction> ReviewReactions { get; set; }
        public DbSet<ReplyReaction> ReplyReactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<UserOffer> UserOffers { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<SecurityLog> SecurityLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.user_id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.room_id);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.booking_id);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Room)
                .WithMany()
                .HasForeignKey(r => r.room_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewReply>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewReply>()
                .HasOne(r => r.Review)
                .WithMany(r => r.Replies)
                .HasForeignKey(r => r.review_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewReply>()
                .HasOne(r => r.ParentReply)
                .WithMany(r => r.ChildReplies)
                .HasForeignKey(r => r.parent_reply_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewReaction>()
                .HasIndex(x => new { x.review_id, x.user_id })
                .IsUnique();

            modelBuilder.Entity<ReplyReaction>()
                .HasOne(x => x.Reply)
                .WithMany(r => r.Reactions)
                .HasForeignKey(x => x.reply_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReplyReaction>()
                .HasIndex(x => new { x.reply_id, x.user_id })
                .IsUnique();
        }
    }
}