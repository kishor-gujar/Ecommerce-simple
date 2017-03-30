using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int Number { get; set; }
        public string Date { get; set; }
        public DateTime RequireDate { get; set; }
        public DateTime ShipDate { get; set; }
        public float SalesTax { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TransactStatus { get; set; }
        public string ErrorLLoc { get; set; }
        public string ErrorMsg { get; set; }
        public string Fulfilled { get; set; }
        public bool Deleted { get; set; }
        public bool Paid { get; set; }
        public string PaymentDate { get; set; }
        public string UserId { get; set; }
        public Payment Payment { get; set; }
        public Shipper Shipper { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}