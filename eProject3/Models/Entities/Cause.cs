using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class Cause
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
