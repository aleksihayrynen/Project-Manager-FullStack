using MongoDB.Bson;

namespace ProjectManager.Models
{
    public class ProjectMembers
    {
        public ObjectId UserId { get; set; }
        public string Role { get; set; } //Roles we give to the User
    }
}
