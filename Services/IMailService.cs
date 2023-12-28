namespace asp.net.Services
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }
}
