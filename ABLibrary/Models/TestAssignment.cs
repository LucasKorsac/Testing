using System;

namespace ABLibrary.Models
{

    public class TestAssignment
    {
        public string TestName { get; set; } = "";

        public string Variant { get; set; } = "";

        public string UserId { get; set; } = "";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}