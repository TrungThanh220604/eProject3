using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eProject3.Controllers
{
    [Authorize(Roles = "client, admin")]
    public class UserController : Controller
    {
        public IActionResult HomePage()
        {
            return View();
        }
    }
}
