using eProject3.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProject3.Models
{
    public class ProjectViewModel
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string? description { get; set; }
        public string? owner { get; set; }
        public string? ownerTel { get; set; }

        public DateTime timestart { get; set; }
        public DateTime timeend { get; set; }

        public string status { get; set; }

        [ForeignKey("Cause")]

        public int cause_id { get; set; }

        public Cause Cause { get; set; }

        public ICollection<Like>? Likes { get; set; }

        public ICollection<Donation>? Donations { get; set; }

        public ICollection<Gallery>? Galleries { get; set; }
        public double TotalAmount { get; set; }
    }
}
