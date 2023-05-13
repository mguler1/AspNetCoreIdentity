using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Controllers
{
	public class OrderController : Controller
	{
		//[Authorize(Policy = "OrderPermissionReadAndDelete")]
		[Authorize(Policy = "Permisson.Order.Read")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
