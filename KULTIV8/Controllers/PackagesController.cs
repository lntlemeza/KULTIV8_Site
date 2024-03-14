using Microsoft.AspNetCore.Mvc;

namespace KULTIV8.Controllers
{
	public class PackagesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
