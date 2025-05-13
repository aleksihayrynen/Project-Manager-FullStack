using MongoDB.Bson;

namespace ProjectManager.Models.ViewModels
{
    public class AllTaskViewModel
    {
        // Task Item information
        public TaskItem Task { get; set; }

        // Project-related information (as properties)
        public string ProjectTitle { get; set; }
        public ObjectId ProjectId { get; set; }
        public bool UseReview { get; set; }
        public bool OpenCreate { get; set; }
        public string UserRole { get; set; }
    }
}
