namespace Quiz_App.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public bool ShowInResults { get; set; }
        public int Order { get; set; }  // To allow reordering questions

        public int TemplateId { get; set; }
        public Template Template { get; set; }

        public ICollection<Option> Options { get; set; } = new List<Option>();
    }

    public enum QuestionType
    {
        SingleLineText,
        MultiLineText,
        PositiveInteger,
        Checkbox
    }

}
