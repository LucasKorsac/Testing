using System;
using System.Collections.Generic;
using System.Text;

namespace ABLibrary.Models
{
    public class EventBatchResponse
    {
        public List<string> AcceptedEventIds { get; set; } = new List<string>();

        public List<string> FailedEventIds { get; set; } = new List<string>();

        public bool Success { get; set; }

        public int Accepted { get; set; }

        public int Failed { get; set; }

        public string Error { get; set; }
    }
}
