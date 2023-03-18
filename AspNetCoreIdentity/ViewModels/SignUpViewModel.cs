using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
            
        }
        [Required(ErrorMessage ="Kullanıcı Adı Boş Bırakılamaz")]
        [Display(Name ="Kullanıcı Adı :")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [EmailAddress(ErrorMessage ="Email Formatı Yanlış")]
        [Display(Name = "Email :")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz")]
        [Display(Name = "Telefon Numarası :")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }
        [Compare(nameof(Password),ErrorMessage ="Şifreler Aynı Değil")]
        [Required(ErrorMessage = "Şifre Tekrar Boş Bırakılamaz")]
        [Display(Name = "Şifre Tekrar :")]
        public string PasswordConfirm { get; set; }
    }
}
