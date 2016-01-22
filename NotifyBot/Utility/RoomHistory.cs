﻿namespace NotifyBot.Utility
{
    using System;
    using System.Collections.Generic;

    using HipchatApiV2;

    using NotifyBot.Models;

    public class RoomHistory
    {
        private readonly HipchatClient client;

        private readonly string roomName;

        public RoomHistory(string roomName)
        {
            this.client = new HipchatClient("z0FtjCYeyGTBsDjnYlohg4fkZlT60NlawQ3VT3Bp");

            this.roomName = roomName;
        }

        public MessageHistory GetRoomMessageHistory(int numPreviousMessages)
        {
            if (numPreviousMessages > 100)
            {
                throw new InvalidOperationException("Cannot get more than 100 messages of room history.");
            }

            var history = this.client.ViewRoomHistory(this.roomName, maxResults: numPreviousMessages);

            var messageHistory = new MessageHistory { MessagesBySender = new List<Tuple<string, string>>() };

            foreach (var record in history.Items)
            {
                messageHistory.MessagesBySender.Add(new Tuple<string, string>(record.From, record.Message));
            }

            return messageHistory;
        }
    }
}