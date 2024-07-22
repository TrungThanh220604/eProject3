using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProject3.Models.Entities
{
    public class Conversation
    {
        [Key]
        public int id { get; set; }

        public string status { get; set; }

        [ForeignKey("User")]
        public string user_id {  get; set; }
        public User User { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}
