using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models
{
    public class TaskItem : DB_SaveableObject
    {
        public ObjectId ProjectId { get; set; }         // Reference to parent project
        public string Title { get; set; }
        public string? Description { get; set; }
        public ObjectId AssignedTo { get; set; }

        public ObjectId CreatedBy { get; set; } // The user's ObjectId
        public bool Completed { get; set; }
        public DateTime DueDate { get; set; }

        public enum TaskState
        {
            InProgress = 0,
            InReview = 1,
            Completed = 2
        }

        public TaskState State { get; set; }



    }
}