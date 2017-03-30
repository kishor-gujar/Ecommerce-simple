using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Shipper
    {
        [Key]
        public int Id { get; set; }

        public string CompanyName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}