using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
        public string Allowed { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}