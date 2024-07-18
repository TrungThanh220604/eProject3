using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eProject3.Models.Entities
{
    public class Donation
    {
        [Key]
        public int id { get; set; }

        public double amount { get; set; }

        public string credit_card { get; set; }

        public string payment_type { get; set; }

        public DateTime expiration_date { get; set; }

        public string CVV { get; set; }

        [ForeignKey("User")]
        public int user_id { get; set; }
        public User User { get; set; }

        [ForeignKey("Project")]
        public int project_id { get; set; }
        public Project Project { get; set; }

    }
}
