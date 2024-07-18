using Microsoft.AspNetCore.Mvc;

namespace eProject3.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
