using eProject3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace eProject3.Controllers
{
    [Authorize(Roles = "client, admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public UserController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            var partner = await dbContext.Partners.ToListAsync();
            ViewBag.Partner = partner;
            return View();
        }

        public async Task<IActionResult> OurPartner()
        {
            var partner = await dbContext.Partners.ToListAsync();
            ViewBag.Partner = partner;
            return View();
        }
        public async Task<IActionResult> OurPartnerDetail(int id)
        {
            var partner = await dbContext.Partners.FindAsync(id);
            ViewBag.Partner = partner;
            return View();
        }

        
    }
}
