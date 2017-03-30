using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
        public string Fulfiled { get; set; }

        public string ShipDate { get; set; }
        public string BillDate { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

    }
}