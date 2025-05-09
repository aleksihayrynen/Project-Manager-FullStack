using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class AllTaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
