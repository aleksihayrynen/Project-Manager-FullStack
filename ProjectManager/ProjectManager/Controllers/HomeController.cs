using Microsoft.AspNetCore.Mvc;
using ProjectManager.Models;
using System.Diagnostics;

namespace ProjectManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult ProfileTest()
        {
            return View(ViewData);

        }
        public IActionResult Index()
        {
            return View(ViewData);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
