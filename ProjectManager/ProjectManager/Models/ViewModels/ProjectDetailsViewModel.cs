using MongoDB.Bson;

namespace ProjectManager.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }
        public List<TaskItem> CompletedTask { get; set; }
        public List<TaskItem> LateTask { get; set; }
        public List<TaskItem> AssignedTask { get; set; }
        public ObjectId CurrentUser { get; set; }


    }
}
