namespace asp.net.Services
{
    public interface IMailService
    {
        Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData);
    }
}
