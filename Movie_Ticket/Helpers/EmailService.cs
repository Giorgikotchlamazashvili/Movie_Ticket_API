using System.Net;
using System.Net.Mail;

namespace Movie_Ticket.Helpers
{
    public class EmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("gkochlamazashvili11@gmail.com", "rhqy ilbu ilji ylpk");
            MailMessage message = new MailMessage();
            message.From = new MailAddress("gkochlamazashvili11@gmail.com");
            message.Subject = subject;
            message.Body = body;
            message.To.Add(to);

            smtpClient.Send(message);

            Console.WriteLine("message send successfully");
        }
    }
}
