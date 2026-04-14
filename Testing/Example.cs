using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing
{
    internal class Example
    {
        public Dictionary<string, int> AB { get; private set; } = new();

        public async Task Init()
        {
            Tests.I.InitTest();

            // Mongo можно использовать через сервис (Facade)
            AB = Tests.I.GetNewAB();
        }
    }
}