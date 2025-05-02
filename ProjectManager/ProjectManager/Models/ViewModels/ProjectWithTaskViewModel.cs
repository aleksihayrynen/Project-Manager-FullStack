namespace ProjectManager.Models.ViewModels
{
    public class ProjectWithTaskViewModel
    {
        public Project Project { get; set; }
        public int TaskCount { get; set; }
        public int TaskInReview { get; set; }
        public int LateTask { get; set; }

        public DateTime? NextTaskDueDate { get; set; }

    }
}
