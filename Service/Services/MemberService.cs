using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using Azure.Core;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;
        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);   
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);
            return (true,null);
        }

        public async Task<(bool, IEnumerable<IdentityError>)> EditUserAsync(UserEditViewModel request, string userName)
        {
            var currentUser =(await _userManager.FindByNameAsync(userName))!;

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthdDay = request.BirthDay;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }
            var updateToUserResult = await _userManager.UpdateAsync(currentUser);
            if (!updateToUserResult.Succeeded)
            {
                return (false, updateToUserResult.Errors);
            }
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            if (request.BirthDay.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthdDay!.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }
            return (true, null);
        }

        public  List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
        {

            return principal.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
        }

        public SelectList GetGenderList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<UserEditViewModel> GetUserEditViewModelAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));
             return  new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                BirthDay = currentUser.BirthdDay,
                City = currentUser.City,
                Gender = currentUser.Gender,
                Phone = currentUser.PhoneNumber!
            };
        }

        public async Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;
            return new UserViewModel
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> PasswordCheck(string userName, string Password)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));

            return await _userManager.CheckPasswordAsync(currentUser, Password);
        }
        
    }
}