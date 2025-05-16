using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProjectManager.Models
{
    public class User : DB_SaveableObject
    {
        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }  

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("salt")]
        public byte[] Salt { get; set; }

        public string ProfilePictureURL { get; set; } = "default.png";



        public string? Occupation { get; set; } = "Not set";
        public string? Organization { get; set; } = "Not set";
        public string? Description { get; set; } = "Not set";
    }
}
