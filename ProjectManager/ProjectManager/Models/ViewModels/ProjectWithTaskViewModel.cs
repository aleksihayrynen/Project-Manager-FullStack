﻿using MongoDB.Bson;

namespace ProjectManager.Models.ViewModels
{
    public class ProjectWithTaskViewModel
    {
        public Project Project { get; set; }
        public int TaskCount { get; set; }
        public int TaskInReview { get; set; }
        public int AssignedTask { get; set; }
        public int LateTask { get; set; }
        public bool IsOwner { get; set; }
        public ObjectId CurrentUser { get; set; }

        public DateTime? NextTaskDueDate { get; set; }

    }
}
