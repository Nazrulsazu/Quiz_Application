using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Here you would integrate your email sending logic.
        // For now, just simulate sending email.
        return Task.CompletedTask;
    }
}
