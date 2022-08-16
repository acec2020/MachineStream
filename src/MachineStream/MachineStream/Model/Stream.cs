namespace MachineStream
{
    public class Stream
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("ref")]
        public string Ref { get; set; }
        [JsonProperty("payload")]
        public Machine Payload { get; set; }
        [JsonProperty("join_ref")]
        public string JoinRef { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
    }
}
