using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quiz_App.Models;
using Quiz_App.ViewModels;

namespace Quiz_App.Controllers
{
    public class FormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FormController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Form/ShowQuestions/5
        [HttpGet]
        public async Task<IActionResult> ShowQuestions(int id)
        {
            // Load template with questions and options
            var template = await _context.Templates
                .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            // Optionally load answers if needed for edit scenario
            var userId = _userManager.GetUserId(User);
            var form = await _context.Forms
                .Include(f => f.Answers)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.TemplateId == id);

            var viewModel = new TemplateFormViewModel
            {
                Template = template,
                Answers = form?.Answers?.ToList() ?? new List<Answer>()  // Include existing answers if any
            };

            return View(viewModel);
        }

        // POST: Form/SubmitAnswers

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(int templateId, List<AnswerViewModel> answers)
        {
            var user = await _userManager.GetUserAsync(User);
            var form = await _context.Forms.FirstOrDefaultAsync(f => f.UserId == user.Id && f.TemplateId == templateId);

            if (form == null)
            {
                form = new Form
                {
                    UserId = user.Id,
                    TemplateId = templateId,
                    SubmittedDate = DateTime.Now
                };

                _context.Forms.Add(form);
                await _context.SaveChangesAsync(); // Save to get form Id
            }

            // Now process and save answers
            foreach (var answerVM in answers)
            {
                var question = await _context.Questions
                    .FirstOrDefaultAsync(q => q.Id == answerVM.QuestionId);

                if (question != null)
                {
                    // Handle checkbox answers (removing existing ones and adding new ones)
                    if (question.Type == QuestionType.Checkbox && answerVM.AnswerValues != null)
                    {
                        var existingAnswers = _context.Answers
                            .Where(a => a.FormId == form.Id && a.QuestionId == answerVM.QuestionId);
                        _context.Answers.RemoveRange(existingAnswers);

                        foreach (var answerValue in answerVM.AnswerValues)
                        {
                            _context.Answers.Add(new Answer
                            {
                                QuestionId = answerVM.QuestionId,
                                AnswerValue = answerValue,
                                FormId = form.Id
                            });
                        }
                    }
                    else
                    {
                        // Handle single-value answers
                        var existingAnswer = await _context.Answers
                            .FirstOrDefaultAsync(a => a.FormId == form.Id && a.QuestionId == answerVM.QuestionId);

                        if (existingAnswer != null)
                        {
                            existingAnswer.AnswerValue = answerVM.AnswerValue;
                        }
                        else
                        {
                            _context.Answers.Add(new Answer
                            {
                                QuestionId = answerVM.QuestionId,
                                AnswerValue = answerVM.AnswerValue,
                                FormId = form.Id
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("FormSubmitted", new { templateId });
        }



        [HttpGet]
        public async Task<IActionResult> FormSubmitted(int templateId)
        {
            var userId = _userManager.GetUserId(User);

            // Retrieve the form and the associated answers for the current user
            var form = await _context.Forms
                .Include(f => f.Answers)
                .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.TemplateId == templateId);

            if (form == null)
            {
                return NotFound();
            }

            // Load the template
            var template = await _context.Templates
                .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null)
            {
                return NotFound();
            }

            // Create a view model to display the answers
            var viewModel = new TemplateFormViewModel
            {
                Template = template,
                Answers = form.Answers.ToList()
            };

            return View(viewModel);
        }
    }

}
