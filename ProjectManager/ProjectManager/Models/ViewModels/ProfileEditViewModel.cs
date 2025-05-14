

using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        [MaxLength(300, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }

        [MaxLength(100, ErrorMessage = "Occupation cannot exceed 100 characters.")]
        public string? Occupation { get; set; }

        [MaxLength(100, ErrorMessage = "Organization cannot exceed 100 characters.")]
        public string? Organization { get; set; }

    }
}
