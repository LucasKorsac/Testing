namespace ABLibrary
{
    public class ABOptions
    {
        public string StorageKey { get; set; } = "ab_events";

        public bool AutoFlush { get; set; } = false;

        public int BatchSize { get; set; } = 20;

        public int MaxBufferSize { get; set; } = 1000;

        public int RequestTimeoutSeconds { get; set; } = 10;
    }
}