using System.Threading.Tasks;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string fromEmailAddress, string destinationEmailAddress, string emailSubject, string htmlContent);
    }
}
