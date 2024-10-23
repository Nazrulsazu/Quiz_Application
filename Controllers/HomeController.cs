using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quiz_App.Models;
using Quiz_App.ViewModels;
using System.Linq;

namespace Quiz_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get latest templates (showing 5 as an example)
            var latestTemplates = await _context.Templates
                .OrderByDescending(t => t.CreatedDate)
                .Include(t => t.Author)
                .Take(5)
                .ToListAsync();

            // Get top 5 popular templates (based on number of filled forms)
            var popularTemplates = await _context.Templates
     .Include(t => t.FilledForms) // Include FilledForms to load related data
     .Include(t => t.Author) // Include Author if needed
     .OrderByDescending(t => t.FilledForms.Count) // Order by the number of filled forms
     .Take(5)
     .ToListAsync();


            // Get tag cloud
            var tags = await _context.Tags
                .GroupBy(t => t.Name)
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(t => t.Count)
                .ToListAsync();

            // Create view model
            var homeViewModel = new HomeViewModel
            {
                LatestTemplates = latestTemplates,
                PopularTemplates = popularTemplates,
                TagCloud = tags
            };

            return View(homeViewModel);
        }
    }
}
