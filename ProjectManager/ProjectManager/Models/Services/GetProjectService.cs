using MongoDB.Driver;

namespace ProjectManager.Models.Services
{
    public class GetProjectsService
    {
        public async Task<List<Project>> GetProjectsByUserId(string userId)
        {
            var filter = Builders<Project>.Filter.ElemMatch(
            p => p.Members,
            member => member.UserId.ToString() == userId
            );
             return await MongoManipulator.GetAllObjectsByFilter(filter);
        }

    }
}
