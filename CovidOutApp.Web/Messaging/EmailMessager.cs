using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CovidOutApp.Web.Messaging 
{
    public class EmailMessager : BaseMessager<string, string>, IMessager, IEmailSender
    {
        private readonly string smtpHost;
        private readonly string smtpUserame;
        private readonly string smtpPassword;
        private readonly string smtpPort;
        private readonly SmtpDeliveryMethod smtpMethod;
        public string Subject { get; set; } 
        public bool SSLMode = true;
        public EmailMessager(string host, string port, string username, 
                            string password, SmtpDeliveryMethod method= SmtpDeliveryMethod.Network)
        {
            this.smtpHost = host;
            this.smtpUserame = username;
            this.smtpPassword = password;
            this.smtpPort = port;
            this.smtpMethod = method;
        }
        public void Send(string text)
        {
            SmtpClient client = new SmtpClient();

            client.DeliveryMethod = this.smtpMethod;

            if (client.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory){

                var emailDir= Globals.Emails_DIR;

                if (!Directory.Exists(emailDir)){
                    Directory.CreateDirectory(emailDir);
                }

                  client.PickupDirectoryLocation = emailDir;
            }
            else if (client.DeliveryMethod == SmtpDeliveryMethod.Network){
                client.Host = this.smtpHost;
                client.Port = Int32.Parse(this.smtpPort);
                client.Credentials = new NetworkCredential(this.smtpUserame, this.smtpPassword);
                client.EnableSsl = this.SSLMode;
                
            }
          
            if (this.MessageSender == null)
                throw new Exception("Sender cannot be null");

            if (this.Recipients == null || this.Recipients.Count() == 0)
                throw new Exception("Recipients cannot be null");
            
            var mailMessage = new MailMessage();
            mailMessage.IsBodyHtml= true;
            mailMessage.Subject= this.Subject;                
            mailMessage.From= new MailAddress(this.MessageSender);
            mailMessage.Body = text;

            foreach (var recipient in this.Recipients)
            {
                mailMessage.To.Add(recipient);
            }      

            client.Send(mailMessage); 
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            this.Subject = subject;
            this.Recipients = new List<string>(){ email };
            
            return Task.Run(() => this.Send(htmlMessage));
        }
    }
}