using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProject3.Models.Entities
{
    public class Gallery
    {
        [Key]
        public int id { get; set; }

        public string image { get; set; }

        [ForeignKey("Project")]
        public int project_id { get; set; }
        public Project Project { get; set; }
    }
}
