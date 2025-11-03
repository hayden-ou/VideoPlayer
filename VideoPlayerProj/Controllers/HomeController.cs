using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VideoPlayerProj.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VideoPlayerProj.Controllers
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult VideoPlayer(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", fileName);
            if (System.IO.File.Exists(filePath))
            {
                ViewBag.VideoPath = "/videos/" + fileName;
                return View();
            }
            else
            {
                ViewBag.Message = "Video not found.";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult ListVideos()
        {
            var videoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            var videoFiles = Directory.Exists(videoDirectory)
                ? Directory.GetFiles(videoDirectory).Select(Path.GetFileName).ToList()
                : new List<string>();
            return View(videoFiles);
        }
    }
}
