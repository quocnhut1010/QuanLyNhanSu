using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Demo1.Services
{
    public static class EmailService
    {
        public static void SendSafe(string to, string subject, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(to)) return;

                var host = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
                var portStr = ConfigurationManager.AppSettings["SmtpPort"] ?? "587";
                var enableSslStr = ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true";
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var pass = ConfigurationManager.AppSettings["SmtpPass"];
                var from = ConfigurationManager.AppSettings["SmtpFrom"] ?? user;

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(from))
                {
                    return;
                }

                int port = 587;
                int.TryParse(portStr, out port);
                bool enableSsl = true;
                bool.TryParse(enableSslStr, out enableSsl);

                using (var client = new SmtpClient(host, port))
                using (var message = new MailMessage(from, to))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(user, pass);

                    message.Subject = subject ?? string.Empty;
                    message.Body = body ?? string.Empty;
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
            catch
            {
                // ignore errors to avoid breaking user flow
            }
        }
    }
}

