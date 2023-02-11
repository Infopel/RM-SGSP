using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SGSP.Dtos;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailDtos _emailDtos;
        public EmailRepository(IOptions<EmailDtos> options)
        {
            _emailDtos = options.Value;
        }
        public async Task SendEMailAsync(EmailRequest emailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailDtos.Mail);
            email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
            email.Subject = emailRequest.Subject;
            var builder = new BodyBuilder();

            if(emailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach(var file in emailRequest.Attachments)
                {
                    if(file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = emailRequest.Body;
            email.Body = builder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.CheckCertificateRevocation = false;
                    await client.ConnectAsync(_emailDtos.Host, _emailDtos.Port, _emailDtos.UseSSL);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailDtos.Mail, _emailDtos.Password);
                    await client.SendAsync(email);
                    client.Disconnect(true);

                }
                catch (Exception e)
                {

                    string a = e.Message;
                }
            }

        }
    }
}
