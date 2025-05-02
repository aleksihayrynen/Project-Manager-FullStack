using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectManager.Models.ViewModels;
using ProjectManager.Models.Services;
using ProjectManager.Models;
using ProjectManager.Models.Utils;
using static ProjectManager.Models.TaskItem;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly GetProjectsService _getProjectsService;
        private readonly GetTaskService _getTasksService;

        public ProjectsController(GetProjectsService getProjectsService, GetTaskService getTasksService)
        {
            _getProjectsService = getProjectsService;
            _getTasksService = getTasksService;
        }


        public IActionResult addTaskModal()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return PartialView("_addTaskModal",
                new AddTaskViewModel
                {
                    DueDate = DateTime.Now,
                    AssignedTo = userId

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


        public async Task<IActionResult> AddTask(AddTaskViewModel model, ObjectId project_id)
        {

            // Add some serious null checking and role checking here
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.TaskName))
            {
                if (string.IsNullOrEmpty(model.TaskName))
                    ModelState.AddModelError(nameof(model.TaskName), "Title cannot be empty");

                return PartialView("_addTaskModal", model);
            }
            else
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                        throw new Exception("User not authenticated.");

                    var newTask = new TaskItem
                    {
                        ProjectId = project_id,
                        Title = model.TaskName,
                        Description = model.Description,
                        AssignedTo = new ObjectId(model.AssignedTo),
                        CreatedBy = new ObjectId(userId),
                        Completed = false,
                        DueDate = model.DueDate,
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
                    var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                    if (string.IsNullOrEmpty(userId))
                        throw new Exception("User not authenticated.");

                    var newProject = new Project
                        {
                            Title = model.ProjectName,
                            CreatedAt = DateTime.Now,
                            OpenCreate = model.OpenCreate, 
                            OpenInvite =  model.OpenInvite, 
                            UseReview = model.UseReview,
                            Members = new List<ProjectMembers>
                            {
                                new ProjectMembers
                                {
                                    Username = username,
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
            ViewData["Title"] = $"Project - {UtilityFunctions.CapitalizeFirstLetter(project_title)}";
            var project_id = new ObjectId(id);
            var result = await _getTasksService.GetProjectWithTasks(project_id);
;

            return View(result);
        }


    }
}
