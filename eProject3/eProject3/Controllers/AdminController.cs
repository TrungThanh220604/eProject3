
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eProject3.Data;
using eProject3.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static eProject3.Controllers.AdminController;


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

        public async Task<IActionResult> Dashboard()
        {
            var amount = await _context.Donations.SumAsync(d => d.amount);
            var numOfProj = await _context.Projects.CountAsync();
            var numOfDonation = await _context.Donations.CountAsync();

            ViewBag.Amount = amount;
            ViewBag.NumOfProj = numOfProj;
            ViewBag.NumOfDonation = numOfDonation;
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View();
        }

        //Category
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Causes.ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(cause);
        }

        //Users
        public class UserRoleViewModel
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string Roles { get; set; }
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    FirstName = user.first_name,
                    LastName = user.last_name,
                    Email = user.Email,
                    Address = user.address,
                    PhoneNumber = user.phone_number,
                    Roles = string.Join(", ", roles)
                });
            }
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(userRoles);
        }

        //User ContactUs
        public async Task<IActionResult> UserContactUs()
        {
            var contactus = await _context.ContactUses.ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(contactus);
        }

        //contactUs_Inf
        public async Task<IActionResult> ContactUs_Inf()
        {
            var contactus_inf = await _context.ContactUsEdits.ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(contactUsEdit);
        }

        //Partners
        public async Task<IActionResult> Partners()
        {
            var partners = await _context.Partners.ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(model);
        }

        //Donations
        public async Task<IActionResult> Donations()
        {
            var categories = await _context.Causes.ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CauseId = category.id;
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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

        //Project
        public async Task<IActionResult> Projects()
        {
            var projects = await _context.Projects.Include(p => p.Cause).ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(projects);
        }

        public async Task<IActionResult> AddProject()
        {
            ViewBag.Categories = await _context.Causes.ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View();
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
                TempData["SuccessMessage"] = "Added project successfully!";

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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(model);
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

            foreach (var gallery in project.Galleries)
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects", gallery.image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Galleries.RemoveRange(project.Galleries);

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Delete project successfully!";
            return RedirectToAction("Projects");
        }

        //Edit project
        [HttpGet]
        public async Task<IActionResult> EditProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Galleries)
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
                ProjectImages = project.Galleries.Select(i => new GalleryViewModel
                {
                    Id = i.id,
                    Image = i.image
                }).ToList()
            };

            ViewBag.Categories = _context.Causes.ToList();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(model);
        }

        public class ProjectViewModel
        {
            public int? Id { get; set; }

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

            public List<GalleryViewModel>? ProjectImages { get; set; }
        }

        public class GalleryViewModel
        {
            public int? Id { get; set; }
            public string? Image { get; set; }
            public IFormFile? NewImage { get; set; }
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
                .Include(p => p.Galleries)
                .FirstOrDefaultAsync(p => p.id == id);
            if (project == null)
            {
                return NotFound();
            }

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

            //add image
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
            }

            //edit image
            if (model.ProjectImages != null)
            {
                foreach (var imageViewModel in model.ProjectImages)
                {
                    var existingImage = project.Galleries.FirstOrDefault(i => i.id == imageViewModel.Id);
                    if (existingImage != null)
                    {
                        if (imageViewModel.NewImage != null)
                        {
                            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects");

                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageViewModel.NewImage.FileName);
                            string filePath = Path.Combine(uploadPath, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageViewModel.NewImage.CopyToAsync(fileStream);
                            }

                            if (!string.IsNullOrEmpty(existingImage.image))
                            {
                                var oldFilePath = Path.Combine(uploadPath, existingImage.image);
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }

                            existingImage.image = uniqueFileName;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Update project successfully!";
            return RedirectToAction("Projects");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Galleries.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imageProjects", image.image);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Galleries.Remove(image);
            await _context.SaveChangesAsync();

            return Ok();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.AboutUs = await _context.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await _context.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            var firstImage = aboutUs.AboutUsImages.FirstOrDefault();

            ViewBag.FirstImage = firstImage;
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.AboutUs = await _context.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await _context.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.AboutUs = await _context.AboutUsChilds
                .Where(ac => ac.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.AboutUsImage = await _context.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.AboutUsImage = await _context.AboutUsImages
                .Where(ai => ai.about_id == aboutUs.id)
                .ToListAsync();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.AboutUs = await _context.AboutUsChilds
               .Where(ac => ac.about_id == aboutUs.id)
               .ToListAsync();
            ViewBag.FirstContent = firstContent;
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
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
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(aboutUs);
        }

        //edit aboutUs
        public class AboutUsViewModel
        {
            public int? Id { get; set; }
            public string? Name { get; set; }
            public List<AboutUsChildViewModel>? AboutUsChildren { get; set; }
            public string? SourcePage { get; set; }
            public List<AboutUsImageViewModel>? AboutUsImages { get; set; }
        }

        public class AboutUsImageViewModel
        {
            public int? Id { get; set; }
            public string? Image { get; set; }
            public IFormFile? NewImage { get; set; }
        }

        public class AboutUsChildViewModel
        {
            public int? Id { get; set; }

            [Required(ErrorMessage = "Title is required.")]
            public string? Title { get; set; }

            [Required(ErrorMessage = "Content is required.")]
            public string? Content { get; set; }
        }

        public async Task<IActionResult> EditAboutUs(int id, string sourcePage)
        {
            var aboutUs = await _context.AboutUses
               .Include(a => a.AboutUsChilds)
               .Include(a => a.AboutUsImages)
               .FirstOrDefaultAsync(m => m.id == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            var viewModel = new AboutUsViewModel
            {
                Id = aboutUs.id,
                Name = aboutUs.name,
                AboutUsChildren = aboutUs.AboutUsChilds.Select(c => new AboutUsChildViewModel
                {
                    Id = c.id,
                    Title = c.title,
                    Content = c.content
                }).ToList(),
                SourcePage = sourcePage,

                AboutUsImages = aboutUs.AboutUsImages.Select(i => new AboutUsImageViewModel
                {
                    Id = i.id,
                    Image = i.image
                }).ToList()
            };
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAboutUs(int id, AboutUsViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var aboutUs = await _context.AboutUses
                        .Include(a => a.AboutUsChilds)
                        .Include(a => a.AboutUsImages)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (aboutUs == null)
                    {
                        return NotFound();
                    }

                    if (viewModel.AboutUsChildren != null)
                    {
                        foreach (var child in viewModel.AboutUsChildren)
                        {
                            var existingChild = aboutUs.AboutUsChilds.FirstOrDefault(c => c.id == child.Id);
                            if (existingChild != null)
                            {
                                existingChild.title = child.Title;
                                existingChild.content = child.Content;
                            }
                        }
                    }

                    // Edit image
                    if (viewModel.AboutUsImages != null)
                    {
                        foreach (var imageViewModel in viewModel.AboutUsImages)
                        {
                            var existingImage = aboutUs.AboutUsImages.FirstOrDefault(i => i.id == imageViewModel.Id);
                            if (existingImage != null)
                            {
                                if (imageViewModel.NewImage != null)
                                {
                                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "imageAboutUs");

                                    if (!Directory.Exists(uploadPath))
                                    {
                                        Directory.CreateDirectory(uploadPath);
                                    }

                                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageViewModel.NewImage.FileName);
                                    string filePath = Path.Combine(uploadPath, uniqueFileName);

                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await imageViewModel.NewImage.CopyToAsync(fileStream);
                                    }

                                    if (!string.IsNullOrEmpty(existingImage.image))
                                    {
                                        var oldFilePath = Path.Combine(uploadPath, existingImage.image);
                                        if (System.IO.File.Exists(oldFilePath))
                                        {
                                            System.IO.File.Delete(oldFilePath);
                                        }
                                    }

                                    existingImage.image = uniqueFileName;
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Update AboutUs successfully!";
                    return RedirectToAction(viewModel.SourcePage, new { id = viewModel.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return View(viewModel);
        }

        //Help center
        public class ConversationViewModel
        {
            public int? Id { get; set; }
            public string? UserName { get; set; }
            public string? LastMessage { get; set; }
            public DateTime? LastMessageTime { get; set; }
            public string? Status { get; set; }
        }
        public async Task<int> GetCountOfConversationsWithStatusZeroAsync()
        {
            return await _context.Conversations
                .CountAsync(c => c.status == "0");
        }

        public async Task<IActionResult> HelpCenter()
        {
            //helpcenter
            var conversations = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User)
                .ToListAsync();

            var viewModel = conversations.Select(c => new ConversationViewModel
            {
                Id = c.id,
                UserName = c.User.UserName,
                LastMessage = c.Messages.LastOrDefault()?.message_text,
                LastMessageTime = c.Messages.LastOrDefault()?.sent_at,
                Status = c.status
            }).ToList();
            viewModel = viewModel.OrderByDescending(vm => vm.LastMessageTime).ToList();
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View(viewModel);
        }

        public class MessageViewModel
        {
            public string MessageText { get; set; }
            public DateTime SentAt { get; set; }
            public bool IsMyMessage { get; set; }
        }

        //check admin
        private async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "admin");
        }

        public async Task<IActionResult> ConversationMessages(int id)
        {
            //helpcenter
            var conversations = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User)
                .ToListAsync();

            var viewModel = conversations.Select(c => new ConversationViewModel
            {
                Id = c.id,
                UserName = c.User.UserName,
                LastMessage = c.Messages.LastOrDefault()?.message_text,
                LastMessageTime = c.Messages.LastOrDefault()?.sent_at,
                Status = c.status
            }).ToList();
            viewModel = viewModel.OrderByDescending(vm => vm.LastMessageTime).ToList();

            //ConversationMessages
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.id == id);

            if (conversation == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel2 = new List<MessageViewModel>();
            foreach (var message in conversation.Messages)
            {
                var isMyMessage = message.user_id == currentUserId;
                if (!isMyMessage)
                {
                    isMyMessage = await IsUserAdminAsync(message.user_id);
                }

                viewModel2.Add(new MessageViewModel
                {
                    MessageText = message.message_text,
                    SentAt = message.sent_at,
                    IsMyMessage = isMyMessage
                });
            }

            ViewBag.Messages = viewModel2;
            ViewBag.ConversationId = id;
            ViewBag.Status = conversation.status;
            ViewBag.UserName = conversation.User.UserName;
            ViewBag.CountStatusZero = await GetCountOfConversationsWithStatusZeroAsync();
            return View("HelpCenter", viewModel);
        }

        //Admin Reply
        [HttpPost]
        public async Task<IActionResult> SendMessage(int conversationId, string messageText)
        {
            if (string.IsNullOrEmpty(messageText))
            {
                return RedirectToAction("ConversationMessages", new { id = conversationId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var message = new Message
            {
                conversation_id = conversationId,
                user_id = userId,
                message_text = messageText,
                sent_at = DateTime.Now
            };

            _context.Messages.Add(message);

            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.id == conversationId);
            if (conversation != null)
            {
                conversation.status = "1";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ConversationMessages", new { id = conversationId, selected = conversationId });
        }

    }
}
