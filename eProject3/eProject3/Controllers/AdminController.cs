using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eProject3.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
