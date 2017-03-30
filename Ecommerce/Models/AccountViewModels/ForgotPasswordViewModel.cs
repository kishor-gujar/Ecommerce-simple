using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}