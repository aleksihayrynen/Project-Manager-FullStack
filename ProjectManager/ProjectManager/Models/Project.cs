using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models
{
    public class Project : DB_SaveableObject
    {
        [BsonElement("title")]

        [Required]
        public string Title { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ProjectMembers> Members { get; set; } = new List<ProjectMembers>();

    }
}