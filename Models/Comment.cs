namespace Quiz_App.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public DateTime CreatedDate { get; set; }
    }


}
