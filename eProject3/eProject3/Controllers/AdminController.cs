
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eProject3.Data;
using eProject3.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace eProject3.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public AdminController(UserManager<User> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
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

            public string? ExistingLogo { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> AddPartners(PartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.logo != null && model.logo.Length > 0)
                {
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "imagePartners");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.logo.FileName);
                    string filePath = Path.Combine(uploadPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.logo.CopyToAsync(fileStream);
                    }

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

            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "imagePartners");

            string filePath = Path.Combine(uploadPath, partner.logo);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Deleted partner successfully!";
            return RedirectToAction(nameof(Partners));
        }

        [HttpGet]
        public async Task<IActionResult> EditPartner(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            var model = new PartnerViewModel
            {
                id = partner.id,
                name = partner.name,
                description = partner.description,
                ExistingLogo = partner.logo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPartner(PartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var partner = await _context.Partners.FindAsync(model.id);
                if (partner == null)
                {
                    return NotFound();
                }

                partner.name = model.name;
                partner.description = model.description;

                if (model.logo != null && model.logo.Length > 0)
                {
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "imagePartners");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.logo.FileName);
                    string filePath = Path.Combine(uploadPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.logo.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(model.ExistingLogo))
                    {
                        var oldFilePath = Path.Combine(uploadPath, model.ExistingLogo);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    partner.logo = uniqueFileName;
                }

                _context.Update(partner);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Edit Partners successfull!";
                return RedirectToAction(nameof(Partners));
            }
            return View(model);
        }

        //Donations
        public async Task<IActionResult> Donations()
        {
            var categories = await _context.Causes.ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> DonationList(int causeId)
        {
            var cause = await _context.Causes
            .Where(c => c.id == causeId)
            .FirstOrDefaultAsync();
            ViewBag.CauseName = cause?.name;

            var projects = await _context.Projects
            .Where(p => p.cause_id == causeId)
            .Include(p => p.Cause)
            .ToListAsync();

            if (projects == null || !projects.Any())
            {
                ViewBag.Message = "There's no project";
            }

            var projectViewModels = new List<DonationViewModel>();

            foreach (var project in projects)
            {
                var firstGalleryImage = await _context.Galleries
                    .Where(g => g.project_id == project.id)
                    .Select(g => g.image)
                    .FirstOrDefaultAsync();

                double totalAmount = await _context.Donations
                    .Where(d => d.project_id == project.id)
                    .SumAsync(d => d.amount);

                var projectViewModel = new DonationViewModel
                {
                    Id = project.id,
                    Name = project.name,
                    Description = project.description,
                    Owner = project.owner,
                    OwnerTel = project.ownerTel,
                    Timestart = project.timestart,
                    Timeend = project.timeend,
                    FirstImage = firstGalleryImage,
                    Amount = totalAmount
                };

                projectViewModels.Add(projectViewModel);
            }

            return View(projectViewModels);

        }

        public async Task<IActionResult> DonationDetail(int proId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.id == proId);
            var firstGalleryImage = await _context.Galleries
                .Where(g => g.project_id == proId)
                .Select(g => g.image)
                .FirstOrDefaultAsync();
            var category = await _context.Causes.FirstOrDefaultAsync(c => c.id == project.cause_id);
            ViewBag.cateName = category.name;
            int numOfDonate = await _context.Donations.Where(d => d.project_id == proId).CountAsync();
            double totalAmount = await _context.Donations
                .Where(d => d.project_id == proId)
                .SumAsync(d => d.amount);

            var projectViewModel = new DonationViewModel
            {
                Id = project.id,
                Name = project.name,
                Description = project.description,
                Owner = project.owner,
                OwnerTel = project.ownerTel,
                Timestart = project.timestart,
                Timeend = project.timeend,
                Status = project.status,
                FirstImage = firstGalleryImage,
                NumberOfDonations = numOfDonate,
                Amount = totalAmount

            };

            ViewBag.userDonation = await _context.Donations
            .Where(d => d.project_id == proId)
            .Include(d => d.User)
            .ToListAsync();

            return View(projectViewModel);
        }

        public class DonationViewModel
        {
            public int Id { get; set; }
            public int? NumberOfDonations { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? Owner { get; set; }
            public string? OwnerTel { get; set; }
            public double? Amount { get; set; }
            public string? Status { get; set; }
            public DateTime Timestart { get; set; }
            public DateTime Timeend { get; set; }
            public string? FirstImage { get; set; }
        }

        //public async Task<IActionResult> getIdUser()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var userId = user?.Id;
        //    var roles = await _userManager.GetRolesAsync(user);

        //    ViewBag.UserId = userId;
        //    ViewBag.Roles = roles;

        //    return View();
        //}


        //Project
        public async Task<IActionResult> Projects()
        {
            var projects = await _context.Projects.Include(p => p.Cause).ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> AddProject()
        {
            ViewBag.Categories = await _context.Causes.ToListAsync();
            return View();
        }

        public class ProjectViewModel
        {
            public int? Id { get; set; } // Thêm Id cho phương thức POST

            [Required(ErrorMessage = "Project name is required.")]
            public string Name { get; set; }

            public string Description { get; set; }

            public string Owner { get; set; }

            [Phone(ErrorMessage = "Invalid phone number.")]
            public string OwnerTel { get; set; }

            [Required(ErrorMessage = "Time Start is required.")]
            public DateTime TimeStart { get; set; }

            [Required(ErrorMessage = "Time End is required.")]
            public DateTime TimeEnd { get; set; }

            [Required(ErrorMessage = "Category is required.")]
            public int CauseId { get; set; }

            public IFormFileCollection? Images { get; set; }

            public List<string>? ExistingImages { get; set; } // Thêm thuộc tính này để lưu trữ danh sách hình ảnh hiện tại

        }


        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentTime = DateTime.Now;
                string status;
                if (currentTime < model.TimeStart)
                {
                    status = "UPCOMING";
                }
                else if (currentTime >= model.TimeStart && currentTime <= model.TimeEnd)
                {
                    status = "STILL WORKING";
                }
                else
                {
                    status = "FINISHED";
                }

                var project = new Project
                {
                    name = model.Name,
                    description = model.Description,
                    owner = model.Owner,
                    ownerTel = model.OwnerTel,
                    timestart = model.TimeStart,
                    timeend = model.TimeEnd,
                    cause_id = model.CauseId,
                    status = status
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                if (model.Images != null && model.Images.Count > 0)
                {
                    foreach (var formFile in model.Images)
                    {
                        if (formFile.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formFile.FileName);
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects", uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }

                            var gallery = new Gallery
                            {
                                image = uniqueFileName,
                                project_id = project.id
                            };

                            _context.Galleries.Add(gallery);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Projects");
            }

            ViewBag.Categories = _context.Causes.ToList();
            return View(model);
        }

        public async Task<IActionResult> ProjectDetail(int proId)
        {
            if (proId == null)
            {
                return NotFound();
            }

            var project = await _context.Galleries.Where(g => g.project_id == proId).ToListAsync();
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // Action to delete project
        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Galleries)
                .FirstOrDefaultAsync(p => p.id == id);

            if (project == null)
            {
                return NotFound();
            }

            // Delete all associated images from the server
            foreach (var gallery in project.Galleries)
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects", gallery.image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Remove galleries from database
            _context.Galleries.RemoveRange(project.Galleries);

            // Remove project from database
            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            return RedirectToAction("Projects");
        }

        //Edit project
        [HttpGet]
        public async Task<IActionResult> EditProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Galleries) // Đưa thông tin hình ảnh vào model
                .FirstOrDefaultAsync(p => p.id == id);
            if (project == null)
            {
                return NotFound();
            }

            var model = new ProjectViewModel
            {
                Id = project.id,
                Name = project.name,
                Description = project.description,
                Owner = project.owner,
                OwnerTel = project.ownerTel,
                TimeStart = project.timestart,
                TimeEnd = project.timeend,
                CauseId = project.cause_id,
                ExistingImages = project.Galleries.Select(g => g.image).ToList() // Thêm danh sách hình ảnh hiện tại
            };

            ViewBag.Categories = _context.Causes.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(ProjectViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Causes.ToList();
                return View(model);
            }

            var project = await _context.Projects
                .Include(p => p.Galleries) // Đưa thông tin hình ảnh vào model
                .FirstOrDefaultAsync(p => p.id == id);
            if (project == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin của dự án
            project.name = model.Name;
            project.description = model.Description;
            project.owner = model.Owner;
            project.ownerTel = model.OwnerTel;
            project.timestart = model.TimeStart;
            project.timeend = model.TimeEnd;
            project.cause_id = model.CauseId;

            var currentTime = DateTime.Now;
            if (currentTime < model.TimeStart)
            {
                project.status = "UPCOMING";
            }
            else if (currentTime >= model.TimeStart && currentTime <= model.TimeEnd)
            {
                project.status = "STILL WORKING";
            }
            else
            {
                project.status = "FINISHED";
            }

            _context.Projects.Update(project);

            // Nếu có hình ảnh mới, xóa tất cả hình ảnh cũ và thêm hình ảnh mới
            if (model.Images != null && model.Images.Count > 0)
            {
                // Xóa tất cả hình ảnh cũ
                var existingImages = _context.Galleries.Where(g => g.project_id == project.id).ToList();
                foreach (var image in existingImages)
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects", image.image);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Galleries.RemoveRange(existingImages);

                // Xử lý hình ảnh mới
                foreach (var formFile in model.Images)
                {
                    if (formFile.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formFile.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects", uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var gallery = new Gallery
                        {
                            image = uniqueFileName,
                            project_id = project.id
                        };

                        _context.Galleries.Add(gallery);
                    }
                }
            }
            // Nếu không có hình ảnh mới, chỉ cập nhật dự án
            else
            {
                // Không làm gì thêm về hình ảnh
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Projects");
        }

        //AboutUs
        public async Task<IActionResult> AboutUs1(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsChilds)
            .Include(a => a.AboutUsImages)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            var firstImage = aboutUs.AboutUsImages.FirstOrDefault();

            ViewBag.FirstImage = firstImage;
            return View(aboutUs);
        }

        public async Task<IActionResult> AboutUs2(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsChilds)
            .Include(a => a.AboutUsImages)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            var firstImage = aboutUs.AboutUsImages.FirstOrDefault();

            ViewBag.FirstImage = firstImage;
            return View(aboutUs);
        }

        public async Task<IActionResult> AboutUs3(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsImages)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            return View(aboutUs);
        }

        public async Task<IActionResult> AboutUs4(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsChilds)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            return View(aboutUs);
        }

        public async Task<IActionResult> AboutUs5(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsChilds)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            return View(aboutUs);
        }

        public async Task<IActionResult> AboutUs6(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsChilds)
            .Include(a => a.AboutUsImages)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            var firstContent = aboutUs.AboutUsChilds.FirstOrDefault();

            ViewBag.FirstContent = firstContent;
            return View(aboutUs);
        }

        public async Task<IActionResult> AboutUs7(int id)
        {
            var aboutUs = await _context.AboutUses
            .Include(a => a.AboutUsChilds)
            .FirstOrDefaultAsync(a => a.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            return View(aboutUs);
        }

    }
}
