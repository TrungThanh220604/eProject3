using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class ContactUs
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string phone { get; set; }

        public string email { get; set; }

        public string Message { get; set; }

    }
}
