namespace NotifyBot.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class MessageHistory
    {
        public List<Tuple<string, string>> MessagesBySender { get; set; }

        public string GetEmailMessage()
        {
            var table = new HtmlTable();

            foreach (var message in this.MessagesBySender)
            {
                var row = new HtmlTableRow();

                var fromCell = new HtmlTableCell { InnerText = message.Item1 };
                var messageCell = new HtmlTableCell { InnerText = message.Item2 };

                row.Cells.Add(fromCell);
                row.Cells.Add(messageCell);

                table.Rows.Add(row);
            }

            using (var writer = new StringWriter())
            {
                table.RenderControl(new HtmlTextWriter(writer));

                return writer.ToString();
            }
        }
    }
}