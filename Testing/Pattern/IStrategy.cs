using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Pattern
{
    public interface IStrategy<T>
    {
        //T Choose(List<T> items, T defaultValue);
        T Choose(List<T> items, T defaultValue, string? instanceId = null);
    }
}
