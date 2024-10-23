namespace Quiz_App.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public bool IsPublic { get; set; }
        public string? ImageUrl { get; set; }
        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Question> Questions { get; set; }= new List<Question>();
        public ICollection<Tag>? Tags { get; set; }
        public ICollection<Form>? FilledForms { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<TemplateLike>? Likes { get; set; } = new List<TemplateLike>();
    }

}
