using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace IssueTracker.Services {
    public class EmailSender : IEmailSender {

        public IConfiguration? config;

        public EmailSender(IConfiguration configuration) { 
            config = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message) {
            if (config != null && config["SendEmails"].Equals("true")) {
                int.TryParse(config["SmtpClient:Port"], out int port);
                var client = new SmtpClient(config["SmtpClient:Address"], port) {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(config["SmtpClient:Email"], config["SmtpClient:Password"])
                };
                var mailmsg = new MailMessage(from: config["SmtpClient:Email"],
                    to: email,
                    subject,
                    message);

                mailmsg.IsBodyHtml = true;

                return client.SendMailAsync(mailmsg);
            } else {
                return Task.CompletedTask;
            }
        }
    }
}
