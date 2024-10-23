using Quiz_App.Models;

namespace Quiz_App.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // For non-checkbox questions
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
        public bool IsDeleted { get; set; } 
        public int Order { get; set; }

        // For checkbox-type questions, hold a list of OptionViewModel
        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

        // Helper property to retrieve just the option values (if needed for UI)
       
    }

    // ViewModel for individual checkbox options
    public class OptionViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
