using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Helpers;

public class ApplyDiscountModel : PageModel
{
    private readonly AppDbContext _context;

    public ApplyDiscountModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Room Room { get; set; }

    public void OnGet(int id)
    {
        Room = _context.Rooms.Find(id);
    }

    public IActionResult OnPost()
    {
        var room = _context.Rooms.Find(Room.room_id);

        if (room == null)
            return RedirectToPage("/Admin/ManageRooms");

        room.discount_percent = Room.discount_percent;

        _context.SaveChanges();

        var userId = HttpContext.Session.GetInt32("UserId");

        LogHelper.Log(
            _context,
            HttpContext,
            userId,
            "APPLY_ROOM_DISCOUNT",
            $"Applied {room.discount_percent}% discount to Room ID {room.room_id}"
        );

        return RedirectToPage("/Admin/ManageRooms");
    }
}