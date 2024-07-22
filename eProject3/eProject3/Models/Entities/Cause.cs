using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class Cause
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        public string name { get; set; }

        public ICollection<Project>? Projects { get; set; }
    }
}
