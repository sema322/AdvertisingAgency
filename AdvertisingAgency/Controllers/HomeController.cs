

using Microsoft.AspNetCore.Mvc;
using AdvertisingAgency.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace AdvertisingAgency.Controllers
{
    [Authorize(Roles = "Admin, Manager")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Category()
        {
            return RedirectToAction("Index", "Category");
        }

        public IActionResult Staff()
        {
            return RedirectToAction("Index", "Staff");
        }

        public IActionResult Client()
        {
            return RedirectToAction("Index", "Client");
        }

        [HttpPost]
        public IActionResult AdvRequest()
        {
            return RedirectToAction("Index", "AdvRequest");
        }

        [HttpPost]
        public IActionResult Advertising()
        {
            return RedirectToAction("Index", "Advertising");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
