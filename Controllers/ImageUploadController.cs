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
    public class ImageUploadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImageUploadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Upload")]
        public IActionResult UploadImage(IFormFile file, string username)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file");
                }

            
                var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

                if (registration == null)
                {
                    return NotFound("User not found");
                }

                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    // Update the properties of the registration entity
                    registration.ImageName = file.FileName;
                    registration.ImageContent = memoryStream.ToArray();

                    _context.SaveChanges();

                    return Ok("Image uploaded successfully");
                }
            }
            catch (DbUpdateException ex)
            {
                // Handle specific database update exception
                return BadRequest($"Error uploading image. Database update error: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading image: {ex.Message}");
            }
        }

         [HttpGet("GetImage/{username}")]
        public IActionResult GetImage(string username)
        {
            var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

            if (registration == null || registration.ImageContent == null)
            {
                return NotFound("Image not found");
            }

            return File(registration.ImageContent, "image/png");
        }

        [HttpDelete("DeleteImage/{username}")]
        public IActionResult DeleteImage(string username)
        {
            try
            {
                var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

                if (registration == null || registration.ImageContent == null)
                {
                    return NotFound("Image not found");
                }
                
                registration.ImageName = null;
                registration.ImageContent = null; // Remove image content
                _context.SaveChanges();

                return Ok("Image deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting image: {ex.Message}");
            }
        }

        [HttpPost("AddComment/{username}")]
        public IActionResult AddComment(string username, [FromBody] string comment)
        {
            var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

            if (registration == null || registration.ImageContent == null)
            {
                return NotFound("Image not found");
            }

            registration.ImageComment = comment;
            _context.SaveChanges();

            return Ok("Comment added successfully");
        }

        [HttpPut("EditComment/{username}")]
        public IActionResult EditComment(string username, [FromBody] string comment)
        {
            var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

            if (registration == null || registration.ImageContent == null)
            {
                return NotFound("Image not found");
            }

            registration.ImageComment = comment;
            _context.SaveChanges();

            return Ok("Comment updated successfully");
        }

        [HttpDelete("DeleteComment/{username}")]
        public IActionResult DeleteComment(string username)
        {
            try
            {
                var registration = _context.Registration.FirstOrDefault(r => r.UserName == username);

                if (registration == null || registration.ImageContent == null)
                {
                    return NotFound("Image not found");
                }

                registration.ImageComment = null; // Remove comment
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
