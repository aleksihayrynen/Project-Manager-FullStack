using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.ViewModels
{
    public class AddProjectViewModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [DisplayName("Project name")]
        public string ProjectName { get; set; }

        [DisplayName("Description")]
        [MaxLength(200)]
        public string? Description { get; set; } = "";

        [Required]
        [DisplayName("Open Create")]
        public bool OpenCreate { get; set; }

        [Required]
        [DisplayName("Open Invite")]
        public bool OpenInvite { get; set; }

        [Required]
        [DisplayName("Use Review")]
        public bool UseReview { get; set; }
    }
}
