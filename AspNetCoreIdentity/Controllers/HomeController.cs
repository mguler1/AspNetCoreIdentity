﻿using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentity.Extensions;

using System.Security.Claims;
using Core.Models;
using AspNetCoreIdentity.Service.Services;

namespace AspNetCoreIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var identitiyResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                Email = request.Email

            }, request.PasswordConfirm);

            if (!identitiyResult.Succeeded)
            {
                ModelState.AddModelErrorList(identitiyResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            var excahangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());//10 gün sonra  sayfaya erişim olmayacak
            var user = await _userManager.FindByNameAsync(request.UserName);//useri elde ettim
            var claimResult = await _userManager.AddClaimAsync(user!, excahangeExpireClaim);//claimi kullanıcıya ekledim

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();
            }
            TempData["SuccessMessage"] = "Üyelik Kayıt İşlemi  Başarılı Şekilde Gerçekleşmiştir.";
            return RedirectToAction(nameof(HomeController.SignUp));



        }

        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			returnUrl ??= Url.Action("Index", "Home");

			var hasUser = await _userManager.FindByEmailAsync(model.Email);

			if (hasUser == null)
			{
				ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
				return View();
			}
			var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);


			if (signInResult.IsLockedOut)
			{
				ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız." });
				return View();
			}

			if (!signInResult.Succeeded)
			{
				ModelState.AddModelErrorList(new List<string>() { $"Email veya şifre yanlış", $"Başarısız giriş sayısı = {await _userManager.GetAccessFailedCountAsync(hasUser)}" });
				return View();
			}

			if (hasUser.BirthdDay.HasValue)
			{
				await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[] { new Claim("birthday", hasUser.BirthdDay.Value.ToString()) });
			}
			return Redirect(returnUrl!);
		}

        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır.");
                return View();
            }

            string passwordResestToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResestToken }, HttpContext.Request.Scheme);


            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Şifre yenileme linki, eposta adresinize gönderilmiştir";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır.");
                return View();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

