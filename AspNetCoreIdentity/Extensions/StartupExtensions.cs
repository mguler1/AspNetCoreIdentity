using AspNetCoreIdentity.CustomValidations;
using AspNetCoreIdentity.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCoreIdentity.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExtension(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz123456789";
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = false;


                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);//3 dk User kilitle
                options.Lockout.MaxFailedAccessAttempts = 3;//3 kez yanlış şifre girilirse
            }).AddPasswordValidator<PasswordValidator>().AddUserValidator<UserValidator>().AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
