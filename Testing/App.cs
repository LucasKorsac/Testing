// File: App.cs
using System;

namespace Testing
{
    internal class App
    {
        public async Task Init()
        {
            var example = new Example();
            await example.Init();

            Controller.I.Init(example.AB);
        }
    }
}
