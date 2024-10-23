using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quiz_App.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Template> Templates { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TemplateLike> TemplateLikes { get; set; } 


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship between Template and Tag
        modelBuilder.Entity<Tag>()
            .HasMany(t => t.Templates)
            .WithMany(t => t.Tags)
            .UsingEntity(j => j.ToTable("TemplateTags"));

        // Template -> Questions relationship
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Template)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TemplateId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for questions when a template is deleted

        // Form -> Template relationship
        modelBuilder.Entity<Form>()
            .HasOne(f => f.Template)
            .WithMany(t => t.FilledForms)
            .HasForeignKey(f => f.TemplateId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for forms when a template is deleted

        // Form -> User relationship
        modelBuilder.Entity<Form>()
            .HasOne(f => f.User)
            .WithMany(u => u.FilledForms)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for forms when a user is deleted

        // Answer -> Question relationship
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict); // No cascade for answers on question deletion

        // Answer -> Form relationship
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Form)
            .WithMany(f => f.Answers)
            .HasForeignKey(a => a.FormId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for answers when a form is deleted

        // Comment -> Author (User) relationship
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for comments when a user is deleted

        // Comment -> Template relationship
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Template)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TemplateId)
            .OnDelete(DeleteBehavior.Restrict); // Restrict delete for comments when a template is deleted

        // TemplateLike -> Template relationship
        modelBuilder.Entity<TemplateLike>()
            .HasOne(tl => tl.Template)
            .WithMany(t => t.Likes)
            .HasForeignKey(tl => tl.TemplateId)
            .OnDelete(DeleteBehavior.Restrict); // Restrict delete for likes when a template is deleted

        // TemplateLike -> User relationship
        modelBuilder.Entity<TemplateLike>()
            .HasOne(tl => tl.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for likes when a user is deleted
    }
}
