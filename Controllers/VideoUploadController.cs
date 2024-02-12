using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Laya.Models;

namespace Laya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoUploadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VideoUploadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Upload")]
        public IActionResult UploadVideo(IFormFile file, string username)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file");
                }

                // Find the registration entity by username
                var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

                if (registration == null)
                {
                    return NotFound("User not found");
                }

                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    // Update the properties of the registration entity
                    registration.VideoName = file.FileName;
                    registration.VideoContent = memoryStream.ToArray();

                    _context.SaveChanges();

                    return Ok("Video uploaded successfully");
                }
            }
            catch (DbUpdateException ex)
            {
                // Handle specific database update exception
                return BadRequest($"Error uploading Video. Database update error: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading Video: {ex.Message}");
            }
        }

         [HttpGet("GetVideo/{username}")]
        public IActionResult GetVideo(string username)
        {
            var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

            if (registration == null || registration.VideoContent == null)
            {
                return NotFound("Video not found");
            }

            return File(registration.VideoContent, "video/mp4");
        }

        [HttpDelete("DeleteVideo/{username}")]
        public IActionResult DeleteVideo(string username)
        {
            try
            {
                var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

                if (registration == null || registration.VideoContent == null)
                {
                    return NotFound("Video not found");
                }
                
                registration.VideoName = null;
                registration.VideoContent = null; // Remove image content
                _context.SaveChanges();

                return Ok("Video deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting video: {ex.Message}");
            }
        }

        [HttpPost("AddComment/{username}")]
        public IActionResult AddComment(string username, [FromBody] string comment)
        {
            var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

            if (registration == null || registration.VideoContent == null)
            {
                return NotFound("Video not found");
            }

            registration.VideoComment = comment;
            _context.SaveChanges();

            return Ok("Comment added successfully");
        }

        [HttpPut("EditComment/{username}")]
        public IActionResult EditComment(string username, [FromBody] string comment)
        {
            var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

            if (registration == null || registration.VideoContent == null)
            {
                return NotFound("Video not found");
            }

            registration.VideoComment = comment;
            _context.SaveChanges();

            return Ok("Comment updated successfully");
        }

        [HttpDelete("DeleteComment/{username}")]
        public IActionResult DeleteComment(string username)
        {
            try
            {
                var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

                if (registration == null || registration.VideoContent == null)
                {
                    return NotFound("Video not found");
                }

                registration.VideoComment = null; // Remove comment
                _context.SaveChanges();

                return Ok("Comment deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting comment: {ex.Message}");
            }
        }
    }
}
