using AspNetCoreIdentity.ClaimsProvider;
using AspNetCoreIdentity.Extensions;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Core.OptionsModel;
using AspNetCoreIdentity.Core.Permisson;
using AspNetCoreIdentity.Requirements;
using AspNetCoreIdentity.Seeds;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options => {
        options.MigrationsAssembly("Repositories");
        });
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
builder.Services.AddScoped<IAuthorizationHandler, ViolanceRequirementHandler>();

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
	options.AddPolicy("OrderPermissionReadAndDelete", policy =>
	{
        policy.RequireClaim("permission", Permisson.Order.Read);
        policy.RequireClaim("permission", Permisson.Order.Delete);
        policy.RequireClaim("permission", Permisson.Stock.Delete);
	});

	options.AddPolicy("Permisson.Order.Read", policy =>
	{
		policy.RequireClaim("permission", Permisson.Order.Read);
	}); 
    
    options.AddPolicy("Permisson.Order.Delete", policy =>
	{
		policy.RequireClaim("permission", Permisson.Order.Delete);
	});
	options.AddPolicy("Permisson.Stock.Delete", policy =>
	{
		policy.RequireClaim("permission", Permisson.Stock.Delete);
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
//seed datayý çaðýrma iþlemi
using (var scope=app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService < RoleManager < AppRole >>();
	await PermissonSeed.Seed(roleManager);
}

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
