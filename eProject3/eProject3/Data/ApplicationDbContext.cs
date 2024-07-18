using eProject3.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace eProject3.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }

        public DbSet<AboutUs> AboutUses { get; set; }
        public DbSet<AboutUsChild> AboutUsChilds { get; set; }
        public DbSet<AboutUsImage> AboutUsImages { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<ContactUs> ContactUses { get; set; }
        public DbSet<ContactUsEdit> ContactUsEdits { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }


    }
}
