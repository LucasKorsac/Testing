using System;
using System.Collections.Generic;
using Testing.Pattern;

namespace Testing
{
    public class Test
    {
        public string Name { get; set; } = "";
        public int Default { get; set; }
        public List<int> Values { get; set; } = new();
        public bool IsActive { get; set; }

        // Использование интерфейса стратегии
        public int GetValue(IStrategy<int> strategy)
        {
            return strategy.Choose(Values, Default);
        }
    }
}
