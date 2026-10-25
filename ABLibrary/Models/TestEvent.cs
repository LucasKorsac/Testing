using System;

namespace ABLibrary.Models
{

    public class TestEvent
    {
        public string EventId { get; set; } = "";

        public string ApplicationId { get; set; } = "";

        public string TestName { get; set; } = "";

        public string Variant { get; set; } = "";

        public string UserId { get; set; } = "";

        public string InstanceId { get; set; } = "";

        public string EventType { get; set; } = "conversion";

        public double Value { get; set; } = 1.0;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}