using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Rol Adı Boş Geçilemez")]
        [Display(Name = "Rol Adı :")]
        public string Name { get; set; } = null!;
    }
}
