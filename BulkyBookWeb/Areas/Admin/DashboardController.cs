using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
