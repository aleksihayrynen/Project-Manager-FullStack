using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectManager.Models.ViewModels;
using ProjectManager.Models.Services;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly GetProjectsService _getProjectsService;
        public ProjectsController(GetProjectsService getProjectsService)
        {
            _getProjectsService = getProjectsService;
        }

        public IActionResult AddItemPartial()
        {
            return PartialView("_addProjectModal", new AddProjectViewModel { });
        }


        public IActionResult Index()
        {

            //    try
            //    {
            //        if (HttpContext != null)
            //        {
            //            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            //            var projects = _getProjectsService.GetProjectsByUserId(userId);
            //            Console.WriteLine(userId);
            //            Console.WriteLine("Print should be users!");
            //            Console.WriteLine(projects.ToJson());
            //            return View(projects); 
            //        }
            //        else
            //        {
            //            throw new InvalidOperationException("HttpContext is null. Unable to proceed.");
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
            //        // Redirect to a generic error page
            //        return RedirectToAction("Error", "Home");
            //    }
            //}

            //public IActionResult test()
            //{
            //    return PartialView("_addProjectModal");
            //}
            return View();
        }
    }
}
