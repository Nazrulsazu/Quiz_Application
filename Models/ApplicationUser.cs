using Microsoft.AspNetCore.Identity;

namespace Quiz_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }

        public ICollection<Template> CreatedTemplates { get; set; }
        public ICollection<Form> FilledForms { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Option> Options { get; set; }
        public ICollection<TemplateLike> Likes { get; set; }
    }
}
