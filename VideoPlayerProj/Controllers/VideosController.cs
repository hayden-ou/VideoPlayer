using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace VideoPlayerProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly ILogger<VideosController> _logger;
        private const long MaxRequestSize = 200L * 1024 * 1024;
        private readonly string _videoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");

        public VideosController(ILogger<VideosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> List()
        {
            Directory.CreateDirectory(_videoDirectory);
            var files = Directory.GetFiles(_videoDirectory)
                .Select(f => new
                {
                    fileName = Path.GetFileName(f),
                    size = new FileInfo(f).Length
                })
                .OrderBy(f => f.fileName)
                .ToArray();

            return Ok(files);
        }

        [HttpPost]
        [RequestSizeLimit(MaxRequestSize)]
        public async Task<IActionResult> Create([FromForm] IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest(new { message = "Please select an MP4 file to upload." });

            var ext = Path.GetExtension(videoFile.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !ext.Equals(".mp4", System.StringComparison.OrdinalIgnoreCase))
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                                  new { message = "Only MP4 files are allowed." });

            if (videoFile.Length > MaxRequestSize)
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                                  new { message = $"File size cannot exceed {MaxRequestSize / 1024 / 1024}MB." });

            try
            {
                Directory.CreateDirectory(_videoDirectory);
                var safeName = Path.GetFileName(videoFile.FileName);
                var filePath = Path.Combine(_videoDirectory, safeName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                    await videoFile.CopyToAsync(stream);

                return Created("/api/videos", new
                {
                    uploaded = new[]
                    {
                        new { fileName = safeName, url = $"/videos/{safeName}" }
                    }
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error uploading video");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new { message = "An error occurred while saving the file." });
            }
        }

        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            var safeName = Path.GetFileName(fileName);
            if (string.IsNullOrWhiteSpace(safeName))
                return BadRequest(new { message = "Invalid file name." });

            var filePath = Path.Combine(_videoDirectory, safeName);
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
    }
}