namespace MachineStream
{
    public class Machine
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("machine_id")]
        public string MachineId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }
}
