namespace ABLibrary.Core
{
    public class ABOptions
    {
        public string StorageKey { get; set; } = "ab_events";

        public bool AutoFlush { get; set; } = true;
    }
}