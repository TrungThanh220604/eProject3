using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class ContactUsEdit
    {
        [Key]
        public int id { get; set; }

        public string title { get; set; }

        public string description { get; set; }
    }
}
