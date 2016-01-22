namespace NotifyBot.Models
{
    using Newtonsoft.Json;

    public class Notification
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Recipients { get; set; }
    }
}