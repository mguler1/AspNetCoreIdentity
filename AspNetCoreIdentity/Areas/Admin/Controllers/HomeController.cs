using AspNetCoreIdentity.Areas.Admin.Models;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();
            var userViewListModel = userList.Select(x => new UserListViewModel()
            {
                UserId = x.Id,
                UserMail = x.Email,
                UserName = x.UserName
            }).ToList();
            return View(userViewListModel);
        }
    }
}
