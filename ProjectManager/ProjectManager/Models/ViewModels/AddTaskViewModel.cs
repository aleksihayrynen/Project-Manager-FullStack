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
        public string TaskName { get; set; } = "Why so many 0's";

        [DisplayName("Description")]
        [MaxLength(200)]
        public string? Description { get; set; } = "";

        [Required]
        [DisplayName("Assigned To")]
        public ObjectId AssignedTo { get; set; }

        [Required]
        public DateTime DueDate { get; set; }


    }
}
