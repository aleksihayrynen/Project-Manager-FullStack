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

        public async Task <List<TaskItem>> GetUsersTasksByProjectId(ObjectId project_id, ObjectId userId)
        {
            var filter = Builders<TaskItem>.Filter.And(
            Builders<TaskItem>.Filter.Eq("ProjectId" , project_id),
            Builders<TaskItem>.Filter.Eq("AssignedTo", userId)
            );

            return await MongoManipulator.GetAllObjectsByFilter(filter);
        }

        public async Task <List<TaskItem>> GetTasksByUserId(ObjectId user_id)
        {
            var filter = Builders<TaskItem>.Filter.Eq("AssignedTo", user_id);

            return await MongoManipulator.GetAllObjectsByFilter(filter);
        }

        public async Task UpdateTask(TaskItem task)
        {
            await MongoManipulator.Save(task);
        }

        public async Task DeleteTask(TaskItem task)
        {
            await MongoManipulator.DeleteHelper(task);
        }



        public async Task<ProjectDetailsViewModel> GetProjectWithTasks(ObjectId project_id, ObjectId user_id)
        {
            var project = await MongoManipulator.GetObjectById<Project>(project_id);
            var allTasks = await GetTasksByProjectId(project_id);

            var now = DateTime.UtcNow;

            return new ProjectDetailsViewModel
            {
                Project = project,
                CompletedTask = allTasks.Where(t => t.State == TaskState.Completed).ToList(),
                LateTask = allTasks.Where(t => t.State == TaskState.InProgress && t.DueDate.ToLocalTime() < DateTime.Today).ToList(),
                AssignedTask = allTasks.Where(t => t.State == TaskState.InProgress).ToList(),
                CurrentUser = user_id
            };
        }



        public async Task<List<AllTaskViewModel>> GetAllTasksForUser(ObjectId userId)
        {
            // Fetch all tasks assigned to the user
            var taskFilter = Builders<TaskItem>.Filter.Eq("AssignedTo", userId);
            var userTasks = await MongoManipulator.GetAllObjectsByFilter(taskFilter);

            // If no tasks found, return an empty list
            if (userTasks.Count == 0)
                return new List<AllTaskViewModel>();

            // Fetch all related projects based on the project IDs from the tasks
            var projectIds = userTasks.Select(t => t.ProjectId).Distinct().ToList();
            var projectFilter = Builders<Project>.Filter.In("_id", projectIds);
            var relatedProjects = await MongoManipulator.GetAllObjectsByFilter(projectFilter);

            // Create a mapping of ProjectId to Project and User Role in that project
            var projectRoleMap = relatedProjects.ToDictionary(
                p => p._id,
                p => p.Members.ToDictionary(m => m.UserId, m => m.Role) // Map UserId -> Role for each project
            );

            // Create view models for each task
            var taskViewModels = userTasks.Select(task =>
            {
                // Get the related project
                var project = relatedProjects.First(p => p._id == task.ProjectId);

                // Get the user's role in this project using the role map
                var role = projectRoleMap.ContainsKey(task.ProjectId) && projectRoleMap[task.ProjectId].ContainsKey(userId)
                    ? projectRoleMap[task.ProjectId][userId]
                    : "No role"; // Default to "Viewer" if the role is not found

                // Create the AllTaskViewModel and populate the task and project properties
                return new AllTaskViewModel
                {
                    Task = task, // Full TaskItem
                    ProjectTitle = project.Title,
                    ProjectId = project._id,
                    UseReview = project.UseReview,
                    OpenCreate = project.OpenCreate,
                    UserRole = role
                };
            }).ToList();

            return taskViewModels;
        }






    }

}
