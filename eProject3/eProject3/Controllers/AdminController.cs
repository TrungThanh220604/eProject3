using eProject3.Data;
using eProject3.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace eProject3.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        //Category
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Causes.ToListAsync();
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
                TempData["SuccessMessage"] = "Added category successfully!";
                return RedirectToAction(nameof(Categories));
            }
            return View(cause); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Causes.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Causes.Remove(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Deleted category successfully!";
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Causes.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Cause cause)
        {
            if (ModelState.IsValid)
            {
                _context.Update(cause);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Update category successfully!";
                return RedirectToAction(nameof(Categories));
            }
            return View(cause);
        }

        //Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        //User ContactUs
        public async Task<IActionResult> UserContactUs()
        {
            var contactus = await _context.ContactUses.ToListAsync();
            return View(contactus);
        }

        //contactUs_Inf
        public async Task<IActionResult> ContactUs_Inf()
        {
            var contactus_inf = await _context.ContactUsEdits.ToListAsync();
            return View(contactus_inf);
        }

        public async Task<IActionResult> EditContactUs(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactus = await _context.ContactUsEdits.FindAsync(id);
            if (contactus == null)
            {
                return NotFound();
            }
            return View(contactus);
        }

        [HttpPost]
        public async Task<IActionResult> EditContactUs(ContactUsEdit contactUsEdit)
        {
            if (ModelState.IsValid)
            {
                _context.Update(contactUsEdit);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Update ContactUs_info successfully!";
                return RedirectToAction(nameof(ContactUs_Inf));
            }
            return View(contactUsEdit);
        }

        //Partners
        public async Task<IActionResult> Partners()
        {
            var partners = await _context.Partners.ToListAsync();
            return View(partners);
        }

        public IActionResult AddPartners()
        {
            return View();
        }

        public class PartnerViewModel
        {
            public int id { get; set; }

            [Required]
            public string name { get; set; }

            public IFormFile? logo { get; set; }

            public string? description { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AddPartners(PartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.logo != null && model.logo.Length > 0)
                {
                    // Đường dẫn tới thư mục uploads trong wwwroot
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "image");

                    // Đảm bảo thư mục tồn tại
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Tạo tên file duy nhất để tránh ghi đè
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.logo.FileName);
                    string filePath = Path.Combine(uploadPath, uniqueFileName);

                    // Lưu file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.logo.CopyToAsync(fileStream);
                    }

                    // Tạo và lưu đối tượng Partner sau khi upload file
                    Partner partner = new Partner
                    {
                        name = model.name,
                        logo = uniqueFileName,
                        description = model.description
                    };

                    _context.Add(partner);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Add Partners successfull!";
                    return RedirectToAction(nameof(Partners));
                }
                else
                {
                    ModelState.AddModelError("", "Vui lòng chọn một file.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Deleted partner successfully!";
            return RedirectToAction(nameof(Partners));
        }




    }
}
