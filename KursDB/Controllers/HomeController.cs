using Microsoft.AspNetCore.Mvc;

namespace KursDB.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}