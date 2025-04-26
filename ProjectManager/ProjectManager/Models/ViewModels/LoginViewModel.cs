using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectManager.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Username")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
