using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using NotifyBot.Models;
using NotifyBot.Utility;

namespace NotifyBot.Controllers
{
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;

    public class NotifyController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostNotification(NotifyRequest request)
        {
            HttpResponseMessage responseMessage;
            var commandHandler = new CommandHandler();
            try
            {
                var tempString = Parser.SplitOnFirstWord(request.Item.message.message).Item2;
                if (string.IsNullOrEmpty(tempString))
                {
                    throw new Exception("no email distribution alias provided");
                }

                // parse message
                var parsedMessage = Parser.SplitOnFirstWord(tempString);
                
                //get command
                var commandString = parsedMessage.Item1.Trim();
                Command command;
                var commandResult = Enum.TryParse(commandString, true, out command);
                
                //get message
                var message = parsedMessage.Item2;
                var result = "";

                if (commandResult)
                {
                    switch (command)
                    {
                        case Command.Add:
                            commandHandler.Add(message);
                            result = "Added Successfully";
                            break;
                        case Command.Update:
                            result = commandHandler.Update(message);
                            break;
                    }
                }
                else
                {
                    var isHtml = false;
                    var parsedbody = Parser.SplitOnFirstWord(message);
                    var regex = new Regex(@"^\^(\d)*");
                    var match = regex.Match(parsedbody.Item1);
                    if (match.Success)
                    {
                        var regexDigits = new Regex(@"(\d)+");
                        var matchDigits = regexDigits.Match(parsedbody.Item1);
                        if (matchDigits.Success)
                        {
                            var roomHistory = new RoomHistory(request.Item.room.name);

                            isHtml = false;
                            message = roomHistory.GetRoomMessageHistory(int.Parse(matchDigits.Value)+1).GetEmailMessage();
                        }

                        
                    }
                    var senderName = request.Item.message.from.name;
                    var senderMention = request.Item.message.from.mention_name;
                    result = commandHandler.Email(senderName, senderMention, commandString, message, isHtml);
                }

                if (result == null)
                {
                    throw new Exception("error");
                }


                // Notify Hipchat
                var responseBody = new NotifyResponse
                {
                    color = "green",
                    message = "It's going to be sunny tomorrow! (" + result + " )",
                    message_format = "text",
                    notify = "false"
                };
                responseMessage = this.Request.CreateResponse(HttpStatusCode.OK, responseBody);
            }
            catch (Exception ex)
            {
                commandHandler.Dispose();
                var responseBody = new NotifyResponse
                {
                    color = "green",
                    message = ex.Message,
                    message_format = "text",
                    notify = "false"
                };
                responseMessage = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseBody));
            }
            return responseMessage;
        }

        
    }
}
