using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProject3.Models.Entities
{
    public class Message
    {
        [Key]
        public int id { get; set; }

        public string message_text { get; set; }

        public DateTime sent_at { get; set; }

        [ForeignKey("Conversation")]
        public int conversation_id {  get; set; }

        public Conversation Conversation { get; set; }

        [ForeignKey("User")]
        public int user_id { get; set; }
        public User User { get; set; }
    }
}
