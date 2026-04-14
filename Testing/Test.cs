// File: Test.cs
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

        // Используем стратегию вместо хардкода
        public int GetValue(Strategy strategy)
        {
            return strategy.GetVariant(Values, Default);
        }
    }
}
