namespace Quiz_App.Models
{
    public class Form
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public DateTime SubmittedDate { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }



}
