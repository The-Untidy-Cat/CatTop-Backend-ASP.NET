using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace asp.net.Services
{
    public class HTMLMailData
    {
        public string Email { get; set; }
        public string Content { get; set; }

        public string Subject { get; set; }
    }
    public class MailService : IMailService
    {
        private readonly MailSetting _mailSettings;
        public MailService(IOptions<MailSetting> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }

        public async Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);

                    MailboxAddress emailTo = new MailboxAddress(htmlMailData.Email, htmlMailData.Email);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = htmlMailData.Subject;

                    string filePath = Directory.GetCurrentDirectory() + "\\Templates\\index.html";

                    string emailTemplateText = await File.ReadAllTextAsync(filePath);

                    emailTemplateText = emailTemplateText.Replace("{0}", htmlMailData.Content);

                    BodyBuilder emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.HtmlBody = emailTemplateText;
                    emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using (SmtpClient mailClient = new SmtpClient())
                    {
                        await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        await mailClient.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

    }
    public class MailSetting
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
