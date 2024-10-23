namespace Quiz_App.Models
{
    public class Option
    {
        public int Id { get; set; }
        public string Value { get; set; } // This holds the text for the option
        public int QuestionId { get; set; } // FK to the Question
        public Question Question { get; set; }
    }

}
