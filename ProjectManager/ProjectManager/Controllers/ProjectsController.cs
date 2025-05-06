using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectManager.Models.ViewModels;
using ProjectManager.Models.Services;
using ProjectManager.Models;
using ProjectManager.Models.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly GetProjectsService _getProjectsService;
        private readonly GetTaskService _getTasksService;

        public ProjectsController(GetProjectsService getProjectsService, GetTaskService getTasksService)
        {
            _getProjectsService = getProjectsService;
            _getTasksService = getTasksService;
        }


        public IActionResult addTaskModal(ObjectId project_id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var project = _getProjectsService.GetProjectById(project_id);
            var members = project.Result.Members;
            
            return PartialView("_addTaskModal",
                new AddTaskViewModel
                {
                    DueDate = DateTime.Now,
                    AssignedTo = userId,
                    UserInfo = members

                });
        }

        public IActionResult addProjectModal()
        {
            return PartialView("_addProjectModal", new AddProjectViewModel { });
        }

        [HttpGet]
        public IActionResult InviteModal(string projectId)
        {
            var model = new InviteViewModel { ProjectId = projectId };
            return PartialView("_inviteModal", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask(AddTaskViewModel model, ObjectId project_id)
        {

            // Add some serious null checking and role checking here
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.TaskName))
            {
                if (string.IsNullOrEmpty(model.TaskName))
                    ModelState.AddModelError(nameof(model.TaskName), "Title cannot be empty");

                // If a validation error happens need to refetch the userInfo, couldnt pass it from the Razor View directly
                var project = await _getProjectsService.GetProjectById(project_id);
                model.UserInfo = project.Members;

                return PartialView("_addTaskModal", model);
            }
            else
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                        throw new Exception("User not authenticated.");

                    var actualDate = model.DueDate + (DateTime.Now - DateTime.UtcNow); //Fixes time zone erros with DB and application

                    var newTask = new TaskItem
                    {
                        ProjectId = project_id,
                        Title = model.TaskName,
                        Description = model.Description,
                        AssignedTo = new ObjectId(model.AssignedTo),
                        CreatedBy = new ObjectId(userId),
                        Completed = false,
                        CretedDate = DateTime.Now,
                        DueDate = actualDate,
                        State = TaskItem.TaskState.InProgress
                    };
                    await MongoManipulator.Save(newTask);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                    return PartialView("_AddItemPartial", model);
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProject(AddProjectViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.ProjectName))
            {
                if (string.IsNullOrEmpty(model.ProjectName))
                    ModelState.AddModelError(nameof(model.ProjectName), "Title cannot be empty");
                    

                return PartialView("_addProjectModal", model);
            }
            else
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var user = MongoManipulator.GetObjectById<User>(userId);


                    if (string.IsNullOrEmpty(userId))
                        throw new Exception("User not authenticated.");

                    var newProject = new Project
                        {
                            Title = model.ProjectName,
                            Description = model.Description ?? "No description ;(",
                            CreatedAt = DateTime.Now,
                            OpenCreate = model.OpenCreate, 
                            OpenInvite =  model.OpenInvite, 
                            UseReview = model.UseReview,
                            Members = new List<ProjectMembers>
                            {
                                new ProjectMembers
                                {
                                    Username = user.Result.Username,
                                    FirstName = user.Result.FirstName,
                                    LastName = user.Result.LastName,
                                    UserId = new ObjectId(userId),
                                    Role = "Owner"
                                    
                                    
                                }
                            }
                        };
                    await MongoManipulator.Save(newProject);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                    return PartialView("_AddItemPartial", model);
                }
            }
        }

        [Authorize]
        public async  Task<IActionResult> Index()
        {
            
            ViewData["Title"] = "Projects";
            try
            {
                if (HttpContext.User.Claims != null)
                {
                    var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                        throw new InvalidOperationException("User is null. Unable to proceed.");

                    var result = await _getProjectsService.GetProjectsWithTaskInfo(new ObjectId(userId));
                    return View(result);
                }
                else
                {
                    throw new InvalidOperationException("User is null. Unable to proceed.");
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                // Redirect to a generic error page
                return RedirectToAction("Error", "Home");
            }

        }

        public async Task<IActionResult> Details(string id, string project_title)
        
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewData["Title"] = $"Project - {UtilityFunctions.CapitalizeFirstLetter(project_title)}";
            var project_id = new ObjectId(id);
            var result = await _getTasksService.GetProjectWithTasks(project_id , new ObjectId(userId));
;

            return View(result);
        }


    }
}
