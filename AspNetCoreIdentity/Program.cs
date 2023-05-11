using AspNetCoreIdentity.ClaimsProvider;
using AspNetCoreIdentity.Extensions;
using AspNetCoreIdentity.Models;
using AspNetCoreIdentity.Models.OptionsModel;
using AspNetCoreIdentity.Requirements;
using AspNetCoreIdentity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));// Hangi classdan eriþmek istiyorsak bu interface geçersek eriþim saðlayabiliriz.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddIdentityWithExtension();//içerisi extensions klasöründe
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

//policy tanýmlama
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "ankara");
    });


	 options.AddPolicy("ExchangePolicy", policy =>
	 {
         policy.AddRequirements(new ExchangeExpireRequirement());
	 });

	options.AddPolicy("ViolancePolicy", policy =>
	{
		policy.AddRequirements(new ViolanceRequirement() { ThresholdAge=18});
	});

});

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder=new CookieBuilder();
    cookieBuilder.Name = "IdentityCookie";
    opt.LoginPath =new PathString("/Home/SignIn");
    opt.LogoutPath= new PathString("/Member/LogOut");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan=TimeSpan.FromDays(10);//10 gün  geçerli
    opt.SlidingExpiration = true;//cookie sakla her girþte yenilemek için

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
