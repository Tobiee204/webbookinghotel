using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["X-Frame-Options"] = "DENY";
        ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    }
});

app.UseRouting();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    await next();
});

app.UseSession();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // N?u truy c?p vào /Admin
    if (path.StartsWith("/Admin"))
    {
        var role = context.Session.GetString("Role");

        if (role != "admin")
        {
            context.Response.Redirect("/Login");
            return;
        }
    }

    await next();
});

app.MapRazorPages();

app.Run();