using AspNetCoreIdentity.Extensions;
using AspNetCoreIdentity.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});
builder.Services.AddIdentityWithExtension();//i�erisi extensions klas�r�nde

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder=new CookieBuilder();
    cookieBuilder.Name = "IdentityCookie";
    opt.LoginPath =new PathString("/Home/SignIn");
    opt.LogoutPath= new PathString("/Member/LogOut");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan=TimeSpan.FromDays(10);//10 g�n  ge�erli
    opt.SlidingExpiration = true;//cookie sakla her gir�te yenilemek i�in

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
