using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Rol Adı Boş Geçilemez")]
        [Display(Name = "Rol Adı :")]
        public string Name { get; set; }
    }
}
