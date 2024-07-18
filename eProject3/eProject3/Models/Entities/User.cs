using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? address { get; set; }
        public string? phone_number { get; set; }
        public string role { get; set; }

        public ICollection<Conversation> Conversations { get; set; }
        public ICollection<Message> Messages { get; set; }

        public ICollection<Like> Likes { get; set; }

        public ICollection<Donation> Donations { get; set; }
    }
}
