using hk.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hk.Controllers
{
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

        public IActionResult InputsAndBurn()
        {
            return View();
        }

        public IActionResult Landplots()
        {
            return View();
        }

        public IActionResult Avatars()
        {
            return View();
        }

        public IActionResult WaterTowers()
        {
            return View();
        }

        public IActionResult MintNFTS()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}