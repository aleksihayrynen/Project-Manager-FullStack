using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.ViewModels
{
    public class AddTaskViewModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [DisplayName("Task name")]
        public string TaskName { get; set; }

        [DisplayName("Description")]
        [MaxLength(200)]
        public string? Description { get; set; } = "";

        [Required]
        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public List<ProjectMembers> UserInfo { get; set; }


    }
}
