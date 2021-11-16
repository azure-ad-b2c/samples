using System.Threading.Tasks;

namespace AADMagicLinks.Utility
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string from, string to, string subject, string htmlContent);
    }
}
