using eProject3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eProject3.Controllers
{
    [Authorize(Roles = "client, admin")]
    public class AboutUsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public AboutUsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs1()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 1);

            ViewBag.AboutUs = aboutUs;
            ViewBag.AboutUsImage = aboutUs.AboutUsImages.FirstOrDefault();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs2()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 2);

            ViewBag.AboutUs = await dbContext.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await dbContext.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs3()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 3);

            ViewBag.AboutUs = await dbContext.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await dbContext.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs4()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 4);

            ViewBag.AboutUs = await dbContext.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await dbContext.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs5()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 5);

            ViewBag.AboutUs = await dbContext.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await dbContext.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs6()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 6);

            ViewBag.AboutUs = await dbContext.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await dbContext.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AboutUs7()
        {
            var aboutUs = await dbContext.AboutUses
                .Include(a => a.AboutUsChilds)
                .Include(a => a.AboutUsImages)
                .FirstOrDefaultAsync(a => a.id == 7);

            ViewBag.AboutUs = await dbContext.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await dbContext.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            return View();
        }
    }
}
