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
            }).AddPasswordValidator<PasswordValidator>().AddUserValidator<UserValidator>().AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
