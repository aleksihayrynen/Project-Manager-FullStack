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

        public async Task UpdateTask(TaskItem task)
        {
            await MongoManipulator.Save(task);
        }

        public async Task DeleteTask(TaskItem task)
        {
            await MongoManipulator.DeleteHelper(task);
        }


        public async Task<ProjectDetailsViewModel> GetProjectWithTasks(ObjectId project_id)
        {
            var project = await MongoManipulator.GetObjectById<Project>(project_id);
            var allTasks = await GetTasksByProjectId(project_id);

            var now = DateTime.UtcNow;

            return new ProjectDetailsViewModel
            {
                Project = project,
                CompletedTask = allTasks.Where(t => t.State == TaskState.Completed).ToList(),
                LateTask = allTasks.Where(t => t.State == TaskState.InProgress && t.DueDate.ToLocalTime() < DateTime.Today).ToList(),
                AssignedTask = allTasks.Where(t => t.State == TaskState.InProgress).ToList()
            };
        }
    }
    
}
