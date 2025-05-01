using MongoDB.Bson;
using MongoDB.Driver;

namespace ProjectManager.Models.Services
{
    public class GetProjectsService
    {
        public async Task<List<Project>> GetProjectsByUserId(ObjectId userId)
        {
            var filter = Builders<Project>.Filter.ElemMatch(
            p => p.Members,
            member => member.UserId == userId
            );
             return await MongoManipulator.GetAllObjectsByFilter(filter);
        }

        public async Task<Project> GetProjectById(ObjectId id)
        {
            return await MongoManipulator.GetObjectById<Project>(id);
        }

        public async Task<List<User>> GetUsersById(List<string> userIds)
        {
            var objectIds = userIds.Select(id => new ObjectId(id)).ToList();
            var filter = Builders<User>.Filter.In(u => u._id, objectIds);

            return await MongoManipulator.GetAllObjectsByFilter(filter);
        }

        public async Task<List<UserSearchList>> SearchByUsernameOrEmail(string query)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Username, new BsonRegularExpression(query, "i")), //i = case is ignored, Insensitive
                Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression(query, "i")),   //"^" +  Regex.Escape(query), "i"  for starts with
                Builders<User>.Filter.Regex(u => u.FirstName, new BsonRegularExpression(query, "i")),
                Builders<User>.Filter.Regex(u => u.LastName, new BsonRegularExpression(query, "i"))
            );

            var users = await MongoManipulator
                .GetCollection<User>()
                .Find(filter)
                .Limit(10) //Only query for first 10 from the DB
                .ToListAsync();

            return users.Select(u => new UserSearchList
            {
                UserId = u._id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
                
            }).ToList();
        }


        public async Task InviteUsersToProject(string projectId, List<ObjectId> invitedUserIds)
        {
            var projectObjectId = new ObjectId(projectId);
            var project = await MongoManipulator.GetObjectById<Project>(projectObjectId);
            if (project == null) throw new Exception("Project not found");

            // Filter out already-added userIds
            var newUserIds = invitedUserIds
                .Where(id => !project.Members.Any(m => m.UserId == id))
                .ToList();

            if (newUserIds.Count == 0)
                return;

            
            var userFilter = Builders<User>.Filter.In(u => u._id, newUserIds);
            var users = await MongoManipulator.GetAllObjectsByFilter(userFilter);

            foreach (var user in users)
            {
                project.Members.Add(new ProjectMembers
                {
                    UserId = user._id,
                    Username = user.Username ?? "",
                    Role = "Member"
                });
            }

            await MongoManipulator.Save(project);
        }

    }
}
