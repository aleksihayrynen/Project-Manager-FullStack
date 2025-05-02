using MongoDB.Bson;
using MongoDB.Driver;
using ProjectManager.Models.ViewModels;
using static ProjectManager.Models.TaskItem;

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



        public async Task<ProjectWithTaskViewModel> GetProjectWithTasks(ObjectId project_id)
        {
            var project = await MongoManipulator.GetObjectById<Project>(project_id);
            var allTasks = await GetTasksByProjectId(project_id);

            return new ProjectWithTaskViewModel
            {
                Project = project,
                TaskCount = allTasks.Count,
                TaskInReview = allTasks.Count(t => t.State == TaskState.InReview),
                LateTask = allTasks.Count(t => t.DueDate < DateTime.Today),
                NextTaskDueDate = allTasks
                    .Where(t => t.State != TaskState.Completed)
                    .OrderBy(t => t.DueDate)
                    .Select(t => (DateTime?)t.DueDate)
                    .FirstOrDefault()
            };
        }
    }
    
}
