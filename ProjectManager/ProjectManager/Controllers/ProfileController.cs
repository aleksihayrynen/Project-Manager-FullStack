using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Models.Services;
using ProjectManager.Models;
using MongoDB.Bson;
using ProjectManager.Models.ViewModels;
using System.Security.Claims;
using ProjectManager.Encryption;
using System.Security.Cryptography;


namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly GetProjectsService _getProjectsService;
        private readonly GetTaskService _getTasksService;

        public ProfileController(GetProjectsService getProjectsService, GetTaskService getTasksService)
        {
            _getProjectsService = getProjectsService;
            _getTasksService = getTasksService;
        }



        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userObjectId = new ObjectId(userId);

            var user = await MongoManipulator.GetObjectById<User>(userId);
            if (user == null)
                return NotFound();

            var projects = await Task.Run(() => _getProjectsService.GetProjectsByUserId(userObjectId));
            var tasks = await Task.Run(() => _getTasksService.GetTasksByUserId(userObjectId));

            // Filter completed tasks
            var completedAssignedTasks = tasks
            .Where(t => t.State == TaskItem.TaskState.Completed && t.AssignedTo == userObjectId)
            .ToList();

            // Group by ProjectId and find the project with most completed tasks
            var mostCommonGroup = completedAssignedTasks
                .GroupBy(t => t.ProjectId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            var mostCommonProjectId = mostCommonGroup?.Key;
            var mostCommonProjectTitle = mostCommonProjectId != null
                ? projects.FirstOrDefault(p => p._id == mostCommonProjectId)?.Title
                : null;

            var viewModel = new ProfileViewModel
            {
                User = user,
                Total_Projects = projects.Count(),
                Total_Tasks = completedAssignedTasks.Count(),
                MostCommonProject_Title = mostCommonProjectTitle,
                MostCommonProject_Done = mostCommonGroup?.Count() ?? 0
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveProfile([FromBody] ProfileEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "No user ID found";
                    return Unauthorized();
                }

                var user = await MongoManipulator.GetObjectById<User>(userId); // however you get current user
                user.Description = model.Description;
                user.Occupation = model.Occupation;
                user.Organization = model.Organization;

                await MongoManipulator.Save(user); // your own save method


                return Json(new { success = true });

            }
            var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return Json(new { success = false, errors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword()
        {
            var oldPassword = Request.Form["OldPassword"].ToString().Trim();
            var newPassword = Request.Form["NewPassword"].ToString().Trim();
            var confirmPassword = Request.Form["ConfirmPassword"].ToString().Trim();

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords did not match";
                return RedirectToAction("Index");
            }
            else if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["ErrorMessage"] = "Passwords cannot be empty";
                return RedirectToAction("Index");
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "No user ID found";
                    return Unauthorized();
                }
                else
                {
                    var user = await MongoManipulator.GetObjectById<User>(userId);
                    if (user != null)
                    {
                        if (Argon2Helper.VerifyPassword(oldPassword, user.Password, user.Salt))
                        {
                            var newSalt = new byte[16];  // create empty bytes for the salt
                            RandomNumberGenerator.Fill(newSalt);  // Fill the salt with secure random generator
                            var hashedPassword = Argon2Helper.HashPassword(newPassword, newSalt);


                            user.Salt = newSalt;
                            user.Password = hashedPassword;

                            await MongoManipulator.Save(user); // This updates the document

                            TempData["Success"] = "Password changed successfully.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Incorrect password.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No user ID found";
                        return NotFound();
                    }
                }
            }
        }
    }
}
