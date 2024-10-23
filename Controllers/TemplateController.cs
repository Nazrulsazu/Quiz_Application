using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quiz_App.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Quiz_App.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TemplateController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action to show the template and its comments
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Show(int id)
        {
            // Fetch template with related comments and author information
            var template = await _context.Templates
                .Include(t => t.Likes)
                .Include(t => t.Comments)   // Include the comments related to the template
                .ThenInclude(c => c.Author) // Include the author of each comment
                .Include(t => t.Author)     // Include the author of the template
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            // Pass the template to the view
            return View(template);
        }

        // Action to handle comment submission
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitComment(int templateId, string text)
        {
            // Ensure the comment text is not empty
            if (string.IsNullOrWhiteSpace(text))
            {
                ModelState.AddModelError("text", "Comment cannot be empty.");
                return RedirectToAction("Show", new { id = templateId });
            }

            // Fetch the template to ensure it exists
            var template = await _context.Templates.FindAsync(templateId);
            if (template == null)
            {
                return NotFound();
            }

            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Create a new comment
            var comment = new Comment
            {
                TemplateId = templateId,       // Link the comment to the template
                Text = text,                   // Set the text of the comment
                AuthorId = user.Id,            // Link the comment to the current user's ID
                Author = user,                 // Set the current user as the author
                CreatedDate = DateTime.Now     // Set the comment creation time
            };

            // Add and save the comment to the database
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Redirect back to the template display page
            return RedirectToAction("Show", new { id = templateId });
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeTemplate(int templateId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Load the template with likes
            var template = await _context.Templates
                .Include(t => t.Likes) // Include likes
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null) return NotFound();

            // Check if the user has already liked the template
            var existingLike = template.Likes.FirstOrDefault(like => like.UserId == user.Id);

            if (existingLike == null)
            {
                // User hasn't liked the template, so add a like
                var like = new TemplateLike
                {
                    UserId = user.Id,
                    TemplateId = template.Id
                };
                _context.TemplateLikes.Add(like);
            }
            else
            {
                // User has already liked the template, so remove the like
                _context.TemplateLikes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();

            // Redirect back to the Show action to update the page
            return RedirectToAction("Show", new { id = templateId });
        }

    }
}
