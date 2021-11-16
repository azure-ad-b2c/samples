namespace PMVInvite.Services
{
    public interface IEmailService
    {
        void SendMagicLinkEmail(string link, string email, string username);
    }
}