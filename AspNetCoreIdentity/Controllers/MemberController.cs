using AspNetCoreIdentity.Extensions;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using Core.Models;
using Service.Services;

namespace AspNetCoreIdentity.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity!.Name!;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider,IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
        }

        public async Task Logout()
        {
          await _memberService.LogOutAsync();
        }
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _memberService.PasswordCheck(userName,request.PasswordOld))
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
                return View();
            }

           var (isSuccess,errors)=await _memberService.ChangePasswordAsync(userName,request.PasswordOld,request.PasswordNew);

            if (isSuccess)
            {
                ModelState.AddModelErrorList(errors);
                return View();
            }
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir";
            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList =_memberService.GetGenderList();
            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

             var(issuccess,error) = await _memberService.EditUserAsync(request,userName);

            if (!issuccess)
            {
                ModelState.AddModelErrorList(error);
                return View();
            }
            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir";
            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;
            message = "Bu sayfayı görmeye yetkiniz yoktur. Yetki almak için  yöneticiniz ile görüşebilirsiniz.";
            ViewBag.message = message;
            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View(_memberService.GetClaims(User));

        }


        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();

        }

		[Authorize(Policy = "ExchangePolicy")]
		[HttpGet]
		public IActionResult ExchangePage()
		{
			return View();

		}

		[Authorize(Policy = "ViolencePolicy")]
		[HttpGet]
		public IActionResult ViolencePage()
		{
			return View();
		}
	}
}
