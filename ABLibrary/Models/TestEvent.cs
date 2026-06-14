using System;

namespace ABLibrary.Models
{

    public class TestEvent
    {
        public string TestName { get; set; } = "";

        public string Variant { get; set; } = "";

        public string UserId { get; set; } = "";

        public string EventType { get; set; } = "conversion";

        public string InstanceId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}