using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace customer_relations_manager.MessageServices
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return ConfigSendGridAsync(message);
        }

        private Task ConfigSendGridAsync(IdentityMessage message)
        {
            var msg = new SendGridMessage();
            msg.AddTo(message.Destination);
            msg.From = new MailAddress("mads.slotsbo@gmail.com", "IT-minds CRM");
            msg.Subject = message.Subject;
            msg.Text = message.Body;
            msg.Html = message.Body;


            var test = ConfigurationManager.AppSettings["mailAccount"];

            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
                );

            var transportWeb = new Web(credentials);

            return transportWeb.DeliverAsync(msg);
        }
    }
}
