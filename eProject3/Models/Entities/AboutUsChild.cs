using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProject3.Models.Entities
{
    public class AboutUsChild
    {
        [Key]
        public int id { get; set; }

        public string title { get; set; }

        public string content { get; set; }

        [ForeignKey("AboutUs")]
        public int about_id { get; set; }
        public AboutUs AboutUs { get; set; }
    }
}
