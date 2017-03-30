using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Display name")]
        public string DisplayName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }

        public string Description { get; set; }
        public string Occupation { get; set; }

        [DisplayName("Profile picture")]
        public string ProfilePicture { get; set; }

        public Gender Gender { get; set; }

        public ApplicationUser User { get; set; }
    }

    public enum Gender
    {
        Mr,
        Mrs
    }
}