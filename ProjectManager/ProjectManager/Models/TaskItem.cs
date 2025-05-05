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

        [BsonIgnore]
        public string AssignedToString
        {
            get => AssignedTo.ToString();
            set => AssignedTo = new ObjectId(value);
        }



        public ObjectId CreatedBy { get; set; } // The user's ObjectId
        public bool Completed { get; set; }
        public  DateTime CretedDate { get; set; }
        public DateTime DueDate { get; set; }
        

        public enum TaskState
        {
            InProgress = 0,
            InReview = 1,
            Completed = 2
        }

        private TaskState _state;
        [BsonElement("State")]
        public TaskState State
        {
            get => _state;
            set
            {
                _state = value;
                Completed = (_state == TaskState.Completed);
            }
        }



    }
}