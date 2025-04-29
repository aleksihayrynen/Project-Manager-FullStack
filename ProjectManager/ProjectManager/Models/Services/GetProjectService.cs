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

    }
}
