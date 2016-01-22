namespace NotifyBot.Models
{
    using Newtonsoft.Json;

    public class NotifyRequest
    {
        [JsonProperty(PropertyName = "event")]
        public string HcEvent { get; set; }
        public HcItem Item { get; set; }
    }

    public class HcItem 
    {
        public HcMessage message { get; set; }
        public HcRoom room { get; set; }
    }

    public class HcRoom 
    {
        public string name { get; set; }
    }

    public class HcMessage
    {
        public HcFrom from { get; set; }
        public string message { get; set; }

    }

    public class HcFrom
    {
        public string mention_name { get; set; }
        public string name { get; set; }
    }
}