using eProject3.Data;
using eProject3.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace eProject3.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Categories()
        {
            var categories = _context.Causes.ToList();
            return View(categories);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Cause cause)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cause);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            return View(cause); // Return the view with the model to display validation errors
        }


    }
}
