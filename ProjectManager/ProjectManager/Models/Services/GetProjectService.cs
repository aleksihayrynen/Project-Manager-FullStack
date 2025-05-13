using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectManager.Models.ViewModels;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using static ProjectManager.Models.TaskItem;

namespace ProjectManager.Models.Services
{
    public class GetProjectsService
    {


        public async Task<bool> UserValidationAsync(ObjectId userId, ObjectId project_id)
        {
            var project = await MongoManipulator.GetObjectById<Project>(project_id) ?? throw new KeyNotFoundException("Project not found."); ;

            return CheckValidation(userId, project.Members); ;
        }

        private bool CheckValidation(ObjectId userId, List<ProjectMembers>membersList)
        {
            if (membersList.Any(m => m.UserId == userId))
            {
                return true;
            }
                
            else
            {
                return false;
            }
        }



        public async Task<List<Project>> GetProjectsByUserId(ObjectId userId)
        {
            var filter = Builders<Project>.Filter.ElemMatch(
            p => p.Members,
            member => member.UserId == userId
            );
             return await MongoManipulator.GetAllObjectsByFilter(filter);
        }


        public async Task<List<TaskItem>> GetTasksByProjectIds(List<ObjectId> projectIds)
        {
            var filter = Builders<TaskItem>.Filter.In(t => t.ProjectId, projectIds);
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

        public async Task<List<ProjectWithTaskViewModel>> GetProjectsWithTaskInfo(ObjectId userId)
        {
            var projects = await GetProjectsByUserId(userId);
            var projectIds = projects.Select(p => p._id).ToList();

            var allTasks = await GetTasksByProjectIds(projectIds);

            return projects.Select(project =>
            {
                var tasks = allTasks.Where(t => t.ProjectId == project._id).ToList();

                return new ProjectWithTaskViewModel
                {
                    Project = project,
                    TaskCount = tasks.Count,
                    TaskInReview = tasks.Count(t => t.State == TaskState.InReview),
                    AssignedTask = tasks.Count(t => t.State == TaskState.InProgress),
                    LateTask =  tasks.Count(t => t.State == TaskState.InProgress && t.DueDate.ToLocalTime() < DateTime.Today),
                    NextTaskDueDate = tasks
                    .Where(t => t.State != TaskState.Completed)
                    .OrderBy(t => t.DueDate)
                    .Select(t => (DateTime?)t.DueDate)
                    .FirstOrDefault()
                };
            }).ToList();
        }


        public async Task<List<UserSearchList>> SearchByUsernameOrEmail(string query, List<ObjectId> excludeUserIds)
        {

            //Search logic
            var searchFilter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Username, new BsonRegularExpression(query, "i")),
                Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression(query, "i")),
                Builders<User>.Filter.Regex(u => u.FirstName, new BsonRegularExpression("^" + Regex.Escape(query), "i")),
                Builders<User>.Filter.Regex(u => u.LastName, new BsonRegularExpression("^" + Regex.Escape(query), "i"))
            );
            // Add the logic for removing the excluded users
            var combinedFilter = Builders<User>.Filter.And(
                searchFilter,
                Builders<User>.Filter.Nin(u => u._id, excludeUserIds) // Skip these users
            );
            //Query DB for the matching users
            var users = await MongoManipulator
                .GetCollection<User>()
                .Find(combinedFilter)
                .Limit(10)  // Only take the first 10 matches to avoid massive search like "a"
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
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Role = "Member"

                });
            }

            await MongoManipulator.Save(project);
        }

    }
}
