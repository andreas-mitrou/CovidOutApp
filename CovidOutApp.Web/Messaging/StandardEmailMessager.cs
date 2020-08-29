using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace CovidOutApp.Web.Messaging {
    public interface IExtendedEmailSender: IEmailSender {
         SmtpDeliveryMethod Delivery { get; set; }
         string From {get; set;}
         int Port {get;set;}
    } 

    public class StandardEmailMessager : IExtendedEmailSender
    {
        private readonly SMTP Options;
        public SmtpDeliveryMethod Delivery { get; set; } = SmtpDeliveryMethod.Network;
        public string From { get; set; } 
        public int Port {get;set;}
        public StandardEmailMessager(IOptions<SMTP> optionsAccessor) 
            =>  Options = optionsAccessor.Value;
  
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.Run(()=>{
                Execute(email,subject,htmlMessage);
            });
        }

        private void Execute(string toEmail, string subject, string htmlMessage)  {
            
            SmtpClient client = new SmtpClient();

            client.DeliveryMethod = this.Delivery;

            if (client.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory){

                var emailDir= Globals.Emails_DIR;

                if (!Directory.Exists(emailDir)){
                    Directory.CreateDirectory(emailDir);
                }

                  client.PickupDirectoryLocation = emailDir;
            }
            else if (client.DeliveryMethod == SmtpDeliveryMethod.Network){
                client.Host = Options.HOST;
                client.Port = this.Port;
                client.Credentials = new NetworkCredential(Options.USERNAME, Options.PASSWORD);
                client.EnableSsl = true;
                
            }
            
            var mailMessage = new MailMessage();
            mailMessage.IsBodyHtml= true;
            mailMessage.Subject=  subject;                
            mailMessage.From= new MailAddress(this.From);
            mailMessage.Body = htmlMessage;
            mailMessage.To.Add(toEmail);
               
            client.Send(mailMessage); 
        } 


    }

}