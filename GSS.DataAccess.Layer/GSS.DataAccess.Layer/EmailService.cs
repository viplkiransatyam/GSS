using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace GSS.DataAccess.Layer
{
    public class EmailService
    {
        public void SendEmail(EmailModel eMailModel)
        {
            const string SERVER = "relay-hosting.secureserver.net";

            MailMessage oMail = new MailMessage();
            
            string sFrom = "support@shreeyogi.com";
            string sTo = eMailModel.EmailTo.ToString();
            string sSubject = eMailModel.Subject;
            string sBody = eMailModel.BodyInfo;

            try
            {
                var smtp = new SmtpClient
                {
                    Host = SERVER,
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sFrom, "test")
                };

                MailMessage message = new MailMessage();
                message.From = new MailAddress(sFrom);
                message.Body = sBody;
                message.IsBodyHtml = true;
                message.Subject = sSubject;

                string[] arrayTo = sTo.Split(';');
                for (int i = 0; i <= arrayTo.GetUpperBound(0);i++ )
                {
                    message.To.Add(arrayTo[i]);
                }

                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
