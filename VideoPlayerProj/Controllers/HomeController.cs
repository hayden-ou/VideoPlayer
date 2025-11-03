using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VideoPlayerProj.Models;

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

        [HttpPost]
        [RequestSizeLimit(200 * 1024 * 1024)] // 250 MB limit
        public IActionResult UploadVideo(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest(new { message = "Please select a valid file." });
            }

            if (videoFile.ContentType != "video/mp4")
            {
                Response.StatusCode = 415;
                ViewBag.Message = "Unsupported video format. Please upload only MP4 file types.";
                return View("Index");
            }
            try
            {
                // Hacky way to add file directly to repo, in real app would use Azure Blob
                // Using file name as identifier rather than a unique ID
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", videoFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    videoFile.CopyTo(stream);
                }
                ViewBag.Message = "Video uploaded successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video.");
                Response.StatusCode = 500;
                ViewBag.Message = "An error occurred while uploading the video.";
            }


            return View("Index");
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

        [HttpDelete]
        public IActionResult DeleteVideo(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok(new { message = "Video deleted successfully." });
            }
            else
            {
                return NotFound(new { message = "Video not found." });
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
