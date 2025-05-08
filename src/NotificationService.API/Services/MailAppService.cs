﻿using System.Net;
using System.Net.Mail;
using DotNetEnv;
using NotificationService.API.Persistence;

namespace NotificationService.API.Services
{
    public class MailAppService
    {
        public class SmtpSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool EnableSsl { get; set; }
            public string From { get; set; }
        }

        private readonly SmtpSettings _smtpSettings;

        public MailAppService()
        {
            Env.Load();
            _smtpSettings = new SmtpSettings
            {
                Host = Env.GetString("SMTP_HOST"),
                Port = int.TryParse(Env.GetString("SMTP_PORT"), out var port) ? port : 25,
                EnableSsl = bool.TryParse(Env.GetString("SMTP_ENABLESSL"), out var enableSsl) && enableSsl,
                Username = Env.GetString("SMTP_USERNAME"),
                Password = Env.GetString("SMTP_PASSWORD"),
                From = Env.GetString("SMTP_FROM"),
            };
        }

        public async Task SendEmailAsync(EmailArgs emailArgs, IList<Attachment> attachments = null)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From),
                Subject = emailArgs.Subject,
                Body = emailArgs.Body,
                IsBodyHtml = true,
            };
            foreach (var to in emailArgs.Address)
            {
                mailMessage.To.Add(to);
            }

            if (!mailMessage.To.Any())
            {
                return;
            }

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.EnableSsl = _smtpSettings.EnableSsl;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                await smtpClient.SendMailAsync(mailMessage);

            }
            
        }
    }
}
