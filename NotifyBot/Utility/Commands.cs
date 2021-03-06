﻿namespace NotifyBot.Utility
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web;

    using NotifyBot.Models;

    public enum Command
    {
        Add,
        Update
    }
    public class CommandHandler
    {
        private static DocumentDbRepository dataRepository;
        public CommandHandler()
        {
            dataRepository = new DocumentDbRepository();
            dataRepository.Setup();
        }
        public void Add(string message)
        {
            var parsedMessage = Parser.SplitOnFirstWord(message);
            var notification = new Notification { Id = parsedMessage.Item1.Trim(), Type = "email", Recipients = parsedMessage.Item2 };
            dataRepository.CreateDocumentAsync(notification.Id, notification);
        }

        public string Update(string message)
        {
            return "That notification alias doesn't exists";
        }

        public string Email(string senderName, string senderMention, string documentId, string message, bool isHtml)
        {
            var document = dataRepository.GetDocument(documentId);

            if (document == null)
            {
                return "That notification alias doesn't exists";
            }
            //Send emails
            var to = Newtonsoft.Json.JsonConvert.DeserializeObject<Notification>(document.ToString()).Recipients;
            var subject = senderName + " AKA " + senderMention + " has notified you!";
            var body = message;

            this.sendEmail(to, subject, body, isHtml);
            return "Email sent successfully";
        }

        private void sendEmail(string to, string subject, string body, bool isHtml)
        {
            var message = new MailMessage();
            const string FromEmail = "hipchatemailbot@gmail.com";
            const string FromPw = "winlikecharliesheen";
            var toEmail = to;
            message.From = new MailAddress(FromEmail);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(FromEmail, FromPw);

                smtpClient.Send(message.From.ToString(), message.To.ToString(),
                                message.Subject, message.Body);
            }
        }

        public void Dispose()
        {
            dataRepository.Dispose();
        }
    }
}