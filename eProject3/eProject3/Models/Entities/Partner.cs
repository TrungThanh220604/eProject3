using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class Partner
    {
        [Key]

        public int id { get; set; }

        public string name { get; set; }

        public string? logo {  get; set; }

        public string? description { get; set; }
    }
}
