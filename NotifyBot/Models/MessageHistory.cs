namespace NotifyBot.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class MessageHistory
    {
        public List<Tuple<string, string>> MessagesBySender { get; set; }

        public string GetEmailMessage()
        {
            var builder = new StringBuilder();

            foreach (var message in this.MessagesBySender)
            {
                builder.Append(message.Item1);
                builder.AppendLine(": ");
                builder.AppendLine(message.Item2);
                builder.AppendLine("<br>");
            }

            return builder.ToString();
        }
    }
}