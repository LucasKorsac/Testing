using System;

namespace ABLibrary.Models
{

    public class TestAssignment
    {
        public string AssignmentId { get; set; } = "";

        public string ApplicationId { get; set; } = "";

        public string TestName { get; set; } = "";

        public string Variant { get; set; } = "";

        public string UserId { get; set; } = "";

        public string InstanceId { get; set; } = "";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}