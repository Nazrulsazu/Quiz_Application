﻿namespace Quiz_App.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        public int FormId { get; set; }
        public Form? Form { get; set; }
        public string? AnswerValue { get; set; }  // Could be text, number, or checkbox state
    }

}
