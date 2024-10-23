using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz_App.Models; 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quiz_App.ViewModels;
using RestSharp;
using Newtonsoft.Json;

public class ImgurResponse
{
    public bool Success { get; set; }
    public int Status { get; set; }
    public ImgurData Data { get; set; }
}

public class ImgurData
{
    public string Link { get; set; }
}
public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    // Allow non-authenticated users to view quizzes
    [AllowAnonymous]
    public IActionResult Index()
    {
        var quizzes = _context.Templates
            .Include(t => t.Author)
            .ToList();
        return View(quizzes);
    }

    // Restrict creating quizzes to authenticated users
    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Template model, IFormFile ImageUrl, string TagString)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
            {
                Console.WriteLine($"Validation error: {error}");
            }
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        model.AuthorId = user.Id;
        model.Author = user;

        if (!string.IsNullOrWhiteSpace(TagString))
        {
            var tagNames = TagString.Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct();

            model.Tags = new List<Tag>();

            foreach (var tagName in tagNames)
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

                if (existingTag != null)
                {
                    model.Tags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _context.Tags.Add(newTag);
                    model.Tags.Add(newTag);
                }
            }
        }

        // Upload image to Imgur
        if (ImageUrl != null && ImageUrl.Length > 0)
        {
            var client = new RestClient();
            var request = new RestRequest("https://api.imgur.com/3/upload", Method.Post);
            string clientId = _configuration["Imgur:ClientId"];

            request.AddHeader("Authorization", $"Client-ID {clientId}");

            using (var stream = new MemoryStream())
            {
                await ImageUrl.CopyToAsync(stream);
                request.AddFile("image", stream.ToArray(), ImageUrl.FileName);
            }

            var response = await client.ExecuteAsync(request);
            var imgResponse = JsonConvert.DeserializeObject<ImgurResponse>(response.Content);

            if (imgResponse != null && imgResponse.Data != null)
            {
                model.ImageUrl = imgResponse.Data.Link; // Save Imgur URL
            }
        }

        _context.Templates.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = model.Id });
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var template = await _context.Templates
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .Include(t => t.Author)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (template == null)
        {
            return NotFound();
        }

        return View(template);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var template = await _context.Templates
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (template == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (template.AuthorId != user.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return View(template);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Template model, IFormFile ImageUrl, string TagString)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        // Fetch the existing template
        var existingTemplate = await _context.Templates
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTemplate == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (existingTemplate.AuthorId != user.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // Update properties
        existingTemplate.Title = model.Title;
        existingTemplate.Description = model.Description;
        existingTemplate.Topic = model.Topic;
        existingTemplate.IsPublic = model.IsPublic;

        // Handle Tags
        if (!string.IsNullOrWhiteSpace(TagString))
        {
            var tagNames = TagString.Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct()
                .ToList();

            existingTemplate.Tags.Clear(); // Clear existing tags
            foreach (var tagName in tagNames)
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (existingTag != null)
                {
                    existingTemplate.Tags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _context.Tags.Add(newTag);
                    existingTemplate.Tags.Add(newTag);
                }
            }
        }

        // Handle Image Upload
        if (ImageUrl != null && ImageUrl.Length > 0)
        {
            using (var httpClient = new HttpClient())
            {
                string clientId = _configuration["Imgur:ClientId"];
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {clientId}");

                using (var stream = new MemoryStream())
                {
                    await ImageUrl.CopyToAsync(stream);

                    if (stream.Length == 0)
                    {
                        ModelState.AddModelError("ImageUrl", "Uploaded image is empty.");
                        return View(model);
                    }

                    var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(stream.ToArray()), "image", ImageUrl.FileName);

                    var uploadResponse = await httpClient.PostAsync("https://api.imgur.com/3/upload", content);
                    var imgResponseContent = await uploadResponse.Content.ReadAsStringAsync();

                    // Log the raw response for debugging
                    Console.WriteLine($"Imgur upload response: {imgResponseContent}");

                    // Deserialize the response
                    var imgResponse = JsonConvert.DeserializeObject<ImgurResponse>(imgResponseContent);

                    if (imgResponse != null && imgResponse.Success)
                    {
                        existingTemplate.ImageUrl = imgResponse.Data.Link; // Update with new Imgur URL
                        Console.WriteLine($"Image uploaded successfully: {existingTemplate.ImageUrl}");
                    }
                    else
                    {
                        
                        ModelState.AddModelError("ImageUrl", "Image upload failed. Please try again.");
                    }
                }
            }
        }

        _context.Templates.Update(existingTemplate);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = existingTemplate.Id });
    }


    [Authorize]
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var quiz = _context.Templates.Find(id);
        if (quiz == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (quiz.AuthorId != user.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        _context.Templates.Remove(quiz);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // Add Questions Method
    [HttpPost]
    public async Task<IActionResult> AddQuestions(int templateId, List<QuestionViewModel> questions)
    {
        var template = await _context.Templates
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.Id == templateId);  // Ensure the correct template is fetched

        if (template == null)
        {
            return NotFound();
        }

        foreach (var questionVM in questions)
        {
            if (questionVM.IsDeleted)
            {
                // Handle deleted questions
                var questionToDelete = template.Questions.FirstOrDefault(q => q.Id == questionVM.Id);
                if (questionToDelete != null)
                {
                    _context.Questions.Remove(questionToDelete);  // Remove from database
                }
                continue;
            }

            // Handle existing or new questions
            var question = template.Questions.FirstOrDefault(q => q.Id == questionVM.Id);

            if (question == null) // If no existing question is found, create a new one
            {
                if (!string.IsNullOrWhiteSpace(questionVM.Title))
                {
                    question = new Question
                    {
                        TemplateId = templateId,
                        Title = questionVM.Title,
                        Type = questionVM.Type,
                        Order = questionVM.Order,
                        ShowInResults = questionVM.ShowInResults,
                        Description = questionVM.Description,
                        Options = new List<Option>() // Initialize options for all questions (empty for non-checkbox)
                    };

                    template.Questions.Add(question);
                }
            }
            else // Update existing question
            {
                // Only update if there are changes to the fields
                if (question.Title != questionVM.Title ||
                    question.Description != questionVM.Description ||
                    question.Type != questionVM.Type ||
                    question.Order != questionVM.Order ||
                    question.ShowInResults != questionVM.ShowInResults)
                {
                    question.Title = questionVM.Title;
                    question.Description = questionVM.Description;
                    question.Type = questionVM.Type;
                    question.Order = questionVM.Order;
                    question.ShowInResults = questionVM.ShowInResults;
                }
            }

            // Handling options for Checkbox questions only
            if (question.Type == QuestionType.Checkbox)
            {
                // Clear old options if necessary
                question.Options.Clear();

                // Add new options
                foreach (var optionVM in questionVM.Options)
                {
                    if (!string.IsNullOrWhiteSpace(optionVM.Value))
                    {
                        question.Options.Add(new Option { Value = optionVM.Value });
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = templateId });
    }

    [AllowAnonymous]
    public async Task<IActionResult> SearchByTag(string tag)
    {
        var templates = await _context.Templates
            .Include(t => t.Tags)
            .Where(t => t.Tags.Any(tg => tg.Name == tag))
            .ToListAsync();

        return View("Index", templates);  // Reuse the Index view for search results
    }


}


