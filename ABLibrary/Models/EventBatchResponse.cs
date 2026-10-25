using System;
using System.Collections.Generic;
using System.Text;

namespace ABLibrary.Models
{
    public class EventBatchResponse
    {
        public bool Success { get; set; }

        public int Accepted { get; set; }

        public int Failed { get; set; }

        public string Error { get; set; } = "";
    }
}
