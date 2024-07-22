using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class User : IdentityUser
    {
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? address { get; set; }
        public string? phone_number { get; set; }

        public ICollection<Conversation>? Conversations { get; set; }
        public ICollection<Message>? Messages { get; set; }

        public ICollection<Like>? Likes { get; set; }

        public ICollection<Donation>? Donations { get; set; }
    }
}
