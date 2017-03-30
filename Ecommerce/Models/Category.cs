using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Remote("VerifyName", "Categories",
            ErrorMessage = "Category name already exists. Please enter a different Category name.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Category picture")]
        [FileExtensions(Extensions = "jpg, gif, png")]
        public string Picture { get; set; }

        [ScaffoldColumn(false)]
        public bool Active { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}