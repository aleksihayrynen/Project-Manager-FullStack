using MongoDB.Bson;
using MongoDB.Driver;

namespace ProjectManager.Models.Services
{
    public class GetTaskService
    {
        public async Task<List<TaskItem>> GetTasksByProjectId(ObjectId project_id)
        {
            var filter = Builders<TaskItem>.Filter.Eq(
            e => e.ProjectId , project_id
            );
             return await MongoManipulator.GetAllObjectsByFilter(filter);
        }

        public async Task<TaskItem> GetProjectByUserId(ObjectId user_id)
        {
            return await MongoManipulator.GetObjectById<TaskItem>(user_id);
        }

    }
}
