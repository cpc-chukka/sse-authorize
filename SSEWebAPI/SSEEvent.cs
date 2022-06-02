namespace SSEWebAPI
{
    public class SSEEvent
    {
        public string Name { get; set; }
        public object Data { get; set; }
        public string Id { get; set; }
        public int? Retry { get; set; }

        public SSEEvent(string name, object data, string id, int? retry = 1)
        {
            Name = name;
            Data = data;
            Id = id;
            Retry = retry;
        }
    }
}
