using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IMemberService
    {
        Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName);
        Task LogOutAsync();

        Task<bool> PasswordCheck(string userName,string Password);
        Task<(bool, IEnumerable<IdentityError>)> ChangePasswordAsync(string userName,string oldPassword,string newPassword);

        Task<UserEditViewModel> GetUserEditViewModelAsync(string userName);
        SelectList GetGenderList();

        Task<(bool, IEnumerable<IdentityError>)> EditUserAsync(UserEditViewModel request, string userName);

        List<ClaimViewModel> GetClaims(ClaimsPrincipal principal);
    }
}
