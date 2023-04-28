using AspNetCoreIdentity.Areas.Admin.Models;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles =await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            return View(roles);
        }
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            return RedirectToAction(nameof(RoleController.Index));
        }
    }
}
