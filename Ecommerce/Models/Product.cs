using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Quantity per unit")]
        public int QuantityPerUnit { get; set; }

        [DisplayName("Price")]
        [Required]
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Availabe size")]
        public int AvailabelSize { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }

        public decimal Dicount { get; set; }

        [Display(Name = "Unit Weight")]
        public float UnitWeight { get; set; }

        [Display(Name = "Units in stock")]
        public int UnitsInStock { get; set; }

        [ScaffoldColumn(false)]
        public bool Availabel { get; set; }

        [Display(Name = "Discount Availabel")]
        public float DiscountAvailabel { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Product picture")]
        [FileExtensions(Extensions = "jpg, gif, png")]
        public string Picture { get; set; }

        [ScaffoldColumn(false)]
        [Range(0, 5)]
        public int Ranking { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}