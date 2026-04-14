using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    internal class Controller
    {
        public static readonly Controller I = new();

        public Dictionary<string, int> CurrentTests { get; private set; } = new();

        public void Init(Dictionary<string, int> ab)
        {
            CurrentTests = ab;
        }

        public int Get(string name)
        {
            return CurrentTests.TryGetValue(name, out var val) ? val : 0;
        }
    }
}
