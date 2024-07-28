using eProject3.Data;
using eProject3.Models;
using eProject3.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SQLitePCL;
using System.Security.Claims;
using static eProject3.Controllers.AdminController;
using QuickMailer;
using System.Globalization;

namespace eProject3.Controllers
{
    [Authorize(Roles = "client, admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;
        public UserController(ApplicationDbContext dbContext, UserManager<User> userManager, ILogger<UserController> logger)
        {
            this.dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> HomePage()
        {
            var partner = await dbContext.Partners.ToListAsync();
            ViewBag.Partner = partner;
            var projects = await dbContext.Projects
                                  .Include(p => p.Cause)
                                  .Include(p => p.Galleries)
                                  .Include(p => p.Donations)
                                  .Take(4)
                                  .ToListAsync();
            var projectViewModels = new List<Models.ProjectViewModel>();
            foreach (var project in projects)
            {
                double totalAmount = await dbContext.Donations
               .Where(d => d.project_id == project.id)
               .SumAsync(d => d.amount);
                project.Galleries = project.Galleries?.Take(1).ToList();
                projectViewModels.Add(new Models.ProjectViewModel
                {
                    id = project.id,
                    name = project.name,   
                    owner = project.owner,
                    description = project.description,
                    ownerTel = project.ownerTel,
                    timestart = project.timestart,
                    timeend = project.timeend,
                    status = project.status,
                    cause_id = project.cause_id,
                    TotalAmount = totalAmount,
                    Galleries = project.Galleries,
                    Donations = project.Donations,
                    Likes = project.Likes,
                    Cause = project.Cause,
                });
            }
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            ViewBag.Projects = projectViewModels;
            return View();
        }

        public async Task<IActionResult> OurPartner()
        {
            var partner = await dbContext.Partners.ToListAsync();
            ViewBag.Partner = partner;
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            return View();
        }
        public async Task<IActionResult> OurPartnerDetail(int id)
        {
            var partner = await dbContext.Partners.FindAsync(id);
            ViewBag.Partner = partner;
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Category(int? id)
        {
            var cate = await dbContext.Causes.ToListAsync();
            ViewBag.Category = cate;
            if (!id.HasValue || id.Value == 0)
            {
                var projects = await dbContext.Projects
                                      .Include(p => p.Cause)
                                      .Include(p => p.Galleries)
                                      .ToListAsync();
                foreach (var project in projects)
                {
                    project.Galleries = project.Galleries?.Take(1).ToList();
                }
                ViewBag.Projects = projects;
                ViewBag.CategoryTitle = "All Projects";
            }
            else
            {
                var projects = await dbContext.Projects
                                      .Include(p => p.Cause)
                                      .Include(p => p.Galleries)
                                      .Where(p => p.cause_id == id.Value)
                                      .ToListAsync();
                foreach (var project in projects)
                {
                    project.Galleries = project.Galleries?.Take(1).ToList();
                }
                ViewBag.Projects = projects;
                ViewBag.CategoryTitle = cate.FirstOrDefault(c => c.id == id.Value)?.name ?? "Category";
            }
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ProductDetail(int id)
        {
            var projects = await dbContext.Projects
                .Include(p => p.Cause) 
                .Include(p => p.Galleries)
                .Include(p => p.Likes)
                .Include (p => p.Donations)
                .FirstOrDefaultAsync(p => p.id == id); 

            if (projects == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var isLikedByCurrentUser = await dbContext.Likes
                .AnyAsync(l => l.project_id == id && l.user_id == userId);
            var otherProjects = await dbContext.Projects
                .Where(p => p.id != id)  
                .Include(p => p.Cause)
                .Include(p => p.Galleries)
                .Take(4)  
                .ToListAsync();
            var userDonation = await dbContext.Donations
           .Where(d => d.project_id == projects.id)
           .Include(d => d.User)
           .ToListAsync();
            double totalDonations = userDonation.Sum(d => d.amount);
            ViewBag.Projects = projects;
            ViewBag.Galleries = projects.Galleries.ToList();
            ViewBag.OtherProjects = otherProjects;
            var galleriesOther = otherProjects.Select(p => p.Galleries.FirstOrDefault()?.image).ToList();
            ViewBag.GalleriesOther = galleriesOther;
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            ViewBag.IsLikedByCurrentUser = isLikedByCurrentUser;
            ViewBag.Donations = userDonation;
            ViewBag.TotalAmount = totalDonations;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int projectId)
        {
            var userId = _userManager.GetUserId(User);

            var existingLike = await dbContext.Likes
                .FirstOrDefaultAsync(l => l.project_id == projectId && l.user_id == userId);

            if (existingLike != null)
            {
                dbContext.Likes.Remove(existingLike);
            }
            else
            {
                var like = new Like
                {
                    project_id = projectId,
                    user_id = userId
                };
                dbContext.Likes.Add(like);
            }

            await dbContext.SaveChangesAsync();

            return RedirectToAction("ProductDetail", new { id = projectId });
        }

        [HttpPost]
        public async Task<IActionResult> AddContactUs(AddContactUs viewModel)
        {
            var contactUs = new ContactUs
            {
                name = viewModel.name,
                email = viewModel.email,
                phone = viewModel.phone,
                Message = viewModel.Message
            };

            await dbContext.ContactUses.AddAsync(contactUs);
            await dbContext.SaveChangesAsync();
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            return RedirectToAction("HomePage", "User");
        }

        public class ConversationViewModel
        {
            public int? Id { get; set; }
            public string? UserName { get; set; }
            public string? LastMessage { get; set; }
            public DateTime? LastMessageTime { get; set; }
            public string? Status { get; set; }
        }
        private async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "client");
        }
        public class MessageViewModel
        {
            public string MessageText { get; set; }
            public DateTime SentAt { get; set; }
            public bool IsMyMessage { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> HelpCenter()
        {
            var userId = _userManager.GetUserId(User);
            var conversation = await dbContext.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Id == userId);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    user_id = userId,
                    status = "0", 
                };

                dbContext.Conversations.Add(conversation);
                await dbContext.SaveChangesAsync();
            }
            conversation = await dbContext.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Id == userId);
            var viewModel = new ConversationViewModel
            {
                Id = conversation.id,
                UserName = conversation.User.UserName,
                LastMessage = conversation.Messages.LastOrDefault()?.message_text,
                LastMessageTime = conversation.Messages.LastOrDefault()?.sent_at,
                Status = conversation.status
            };
            var viewModel2 = new List<MessageViewModel>();
            if (conversation.Messages != null)
            {
                foreach (var message in conversation.Messages)
                {
                    var isMyMessage = message.user_id == userId;
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
            }
            DateTime currentTime = DateTime.Now;
            ViewBag.CurrentTime = currentTime.ToString("HH:mm");
            ViewBag.Messages = viewModel2;
            ViewBag.ConversationId = conversation.id;
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int conversationId,string messageText)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(messageText))
            {
                return RedirectToAction("HelpCenter");
            }

            var message = new Message
            {
                conversation_id = conversationId,
                user_id = userId,
                message_text = messageText,
                sent_at = DateTime.Now
            };

            dbContext.Messages.Add(message);

            var conversation = await dbContext.Conversations.FirstOrDefaultAsync(c => c.id == conversationId);
            if (conversation != null)
            {
                conversation.status = "0";
            }

            await dbContext.SaveChangesAsync();
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            return RedirectToAction("HelpCenter");
        }

        [HttpPost]
        public async Task<IActionResult> Send(Models.MailMessage mailMessage, int projectId)
        {
            try
            {
                
                var project = await dbContext.Projects
                    .FirstOrDefaultAsync(p => p.id == projectId);

                if (project == null)
                {
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);
                var userName = user?.UserName ?? "Unknown";
                QuickMailer.Email email = new QuickMailer.Email();
                string projectUrl = Url.Action("ProductDetail", "User", new { id = projectId }, Request.Scheme);
                string subject = $"Invitation to Join {project.name}";
                string body = $"You have been invited to join the project '{project.name}' by {userName}. You can view the project details at the following link:\r\n{projectUrl}\r\n\";";
                email.SendEmail(mailMessage.To, Credential.Email, Credential.Password, subject, body);
                return RedirectToAction("ProductDetail", new { id = projectId });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int? id)
        {   
            var project = await dbContext.Projects
                .FirstOrDefaultAsync (p => p.id == id);
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var contactUsEdit = await dbContext.ContactUsEdits.ToListAsync();
            ViewBag.ContactUsEdit = contactUsEdit.ToList();
            ViewBag.Project = project;
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string? message, string cvc, string expiry, double money, string number, string the, int prjId)
        {
            if (string.IsNullOrEmpty(expiry) )
            {
                TempData["ErrorMessage"] = "Invalid expiry. Please check again.";
                return RedirectToAction("Payment", "User");
            }
            DateTime expirationDate;
            string expiryFullFormat = expiry.Trim();

            // Remove any spaces in the expiry input and check if it is in "MM/YY" format
            expiryFullFormat = expiryFullFormat.Replace(" ", "");

            if (expiryFullFormat.Length == 5 && expiryFullFormat[2] == '/')
            {
                string yearPart = expiryFullFormat.Substring(3, 2);
                string monthPart = expiryFullFormat.Substring(0, 2);

                int yearFull;
                if (int.TryParse(yearPart, out int yearShort))
                {
                    yearFull = (yearShort >= 0 && yearShort <= 99) ? (yearShort + 2000) : DateTime.Now.Year;
                }
                else
                {
                    yearFull = DateTime.Now.Year;
                }

                expiryFullFormat = $"{monthPart}/{yearFull}";
            }

            if (!DateTime.TryParseExact(expiryFullFormat, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expirationDate))
            {
                _logger.LogError("Invalid expiration date: {ExpiryFullFormat}", expiryFullFormat);
                TempData["ErrorMessage"] = "Invalid expiration date. Please check again.";
                return RedirectToAction("Payment", "User");
            }

            if (expirationDate <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "The expiration date is in the past. Please check again.";
                return RedirectToAction("Payment", "User");
            }

            if (money <= 0)
            {
                TempData["ErrorMessage"] = "Donation amount must be greater than zero.";
                return RedirectToAction("Payment", "User");
            }

            if (string.IsNullOrEmpty(cvc) || cvc.Length != 3)
            {
                TempData["ErrorMessage"] = "Invalid CVV. Please check again.";
                return RedirectToAction("Payment", "User");
            }

            var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.id == prjId);
            if (project == null)
            {
                TempData["ErrorMessage"] = "Invalid project ID.";
                return RedirectToAction("Payment", "User");
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Payment", "User");
            }

            var donate = new Donation
            {
                amount = money,
                credit_card = number,
                payment_type = the,
                message = message,
                expiration_date = expirationDate,
                CVV = cvc,
                user_id = userId,
                project_id = prjId
            };

            dbContext.Donations.Add(donate);
            await dbContext.SaveChangesAsync();
            ViewBag.Project = project;
            TempData["SuccessMessage"] = "Thank you for your donation";
            return RedirectToAction("HomePage", "User");
        }







    }
}
