using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace AspNetCoreIdentity.Seeds
{
	public class PermissonSeed
	{
		public static async Task Seed(RoleManager<AppRole> roleManager)
		{
			var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
			var hasAdvanceRole = await roleManager.RoleExistsAsync("AdvanceRole");
			var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");
			if (!hasBasicRole)
			{
				await roleManager.CreateAsync(new AppRole()
				{
					Name = "BasicRole"
				});

				var basicRole = await roleManager.FindByNameAsync("BasicRole");
				await AddReadPermisson(basicRole, roleManager);
			}

			if (!hasAdvanceRole)
			{
				await roleManager.CreateAsync(new AppRole()
				{
					Name = "AdvanceRole"
				});

				var basicRole = await roleManager.FindByNameAsync("AdvanceRole");
				await AddReadPermisson(basicRole, roleManager);
				await AddUpdateAndCreatePermisson(basicRole, roleManager);
			}
			if (!hasAdminRole)
			{
				await roleManager.CreateAsync(new AppRole()
				{
					Name = "AdminRole"
				});

				var basicRole = await roleManager.FindByNameAsync("AdminRole");
				await AddReadPermisson(basicRole, roleManager);
				await AddUpdateAndCreatePermisson(basicRole, roleManager);
				await AddDeletePermisson(basicRole, roleManager);
			}

		}

		public static async Task AddReadPermisson(AppRole role, RoleManager<AppRole> roleManager)
		{
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Stock.Read));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Order.Read));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Catalog.Read));
		}


		public static async Task AddUpdateAndCreatePermisson(AppRole role, RoleManager<AppRole> roleManager)
		{
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Stock.Update));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Order.Update));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Catalog.Update));

			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Stock.Create));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Order.Create));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Catalog.Create));
		}

		public static async Task AddDeletePermisson(AppRole role, RoleManager<AppRole> roleManager)
		{
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Stock.Delete));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Order.Delete));
			await roleManager.AddClaimAsync(role, new Claim("Permission", Permisson.Permisson.Catalog.Delete));
		}
	}
}