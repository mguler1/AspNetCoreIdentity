using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Models
{
    public class AppUser:IdentityUser
    {
        public string? City { get; set; }
        public string? Picture { get; set; }
        public DateTime? BirthdDay { get; set; }
        public byte? Gender { get; set; }
    }
}
