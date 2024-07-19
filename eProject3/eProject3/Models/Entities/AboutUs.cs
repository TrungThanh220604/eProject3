using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class AboutUs
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public ICollection<AboutUsChild>? AboutUsChilds { get; set; }
        public ICollection<AboutUsImage>? AboutUsImages { get; set; }

    }
}
