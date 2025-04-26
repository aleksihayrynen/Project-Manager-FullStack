using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [DisplayName("Username")]
        public required string Name { get; set; }

        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required]
        [DisplayName("Password")]
        [MaxLength(99)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password ")]
        [MaxLength(99)]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }






    }
}
