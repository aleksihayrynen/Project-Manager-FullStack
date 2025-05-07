using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models
{
    public class Project : DB_SaveableObject
    {
        [BsonElement("title")]

        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool OpenCreate { get; set; } = false;
        public bool OpenInvite { get; set; } = false;
        public bool UseReview { get; set; } = false;
        public List<ProjectMembers> Members { get; set; } = new List<ProjectMembers>();

    }
}