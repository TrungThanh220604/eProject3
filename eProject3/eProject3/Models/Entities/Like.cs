using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProject3.Models.Entities
{
    public class Like
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("User")]
        public int user_id { get; set; }
        public User User { get; set; }

        [ForeignKey("Project")]
        public int project_id { get; set; }
        public Project Project { get; set; }
    }
}
