using System.Collections.Generic;

namespace ABLibrary.Models
{
    public class ServerConfig
    {
        public string ApplicationId { get; set; } = "";

        public string UserId { get; set; } = "";

        public string InstanceId { get; set; } = "";

        public List<TestConfig> Tests { get; set; } = new List<TestConfig>();
    }
}