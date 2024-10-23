using Quiz_App.Models;

namespace Quiz_App.ViewModels
{
    public class TemplateFormViewModel
    {
        public Template Template { get; set; }
        public List<Answer> Answers { get; set; }
    }
    public class AnswerViewModel
    {
        public int QuestionId { get; set; }
        public string AnswerValue { get; set; }
        public List<string> AnswerValues { get; set; }
    }

}
