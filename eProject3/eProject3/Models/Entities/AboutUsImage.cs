using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class AboutUsImage
    {
        [Key]
        public int id { get; set; }

        public string image { get; set; }

        [ForeignKey("AboutUs")]
        public int about_id { get; set; }
        public AboutUs AboutUs { get; set; }
    }
}
