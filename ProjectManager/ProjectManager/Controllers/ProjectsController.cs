using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectManager.Models.ViewModels;
using ProjectManager.Models.Services;
using ExpenseTracker.Models.Utils;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly GetProjectsService _getProjectsService;
        public ProjectsController(GetProjectsService getProjectsService)
        {
            _getProjectsService = getProjectsService;
        }

        
            public IActionResult addTaskModal()
        {
            var userId = new ObjectId(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
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

        
        public async Task<IActionResult> AddTask(AddTaskViewModel model, ObjectId project_id)
        {

            // Add some serious null checking and role checking here
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.TaskName))
            {
                if (string.IsNullOrEmpty(model.TaskName))
                    ModelState.AddModelError(nameof(model.TaskName), "Title cannot be empty");

                return PartialView("addTaskModal", model);
            }
            else
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                    if (string.IsNullOrEmpty(userId))
                        throw new Exception("User not authenticated.");

                    var newTask = new TaskItem
                    {
                        ProjectId = project_id,
                        Title = model.TaskName,
                        Description = model.Description,
                        AssignedTo = model.AssignedTo,
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

                return PartialView("addProjectModal", model);
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
                    var test = new ObjectId(userId);
                    var projects = await _getProjectsService.GetProjectsByUserId(test);
                    return View(projects);
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
            ViewData["Title"] = project_title;
            var project_id = new ObjectId(id);
            var project = await _getProjectsService.GetProjectById(project_id);

            //if (project == null)
            //    return NotFound();

            return View(project);
        }


    }
}
