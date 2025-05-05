using MongoDB.Bson;

namespace ProjectManager.Models
{
    public class ProjectMembers
    {
        public string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public ObjectId UserId { get; set; }
        public string Role { get; set; } //Roles we give to the User
    }
}
