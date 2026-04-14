using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    internal class Tests
    {
        public static readonly Tests I = new Tests();

        private readonly Strategy _strategy = new Strategy();

        public List<Test> All { get; set; } = new();
        public List<Test> Active { get; set; } = new();

        public void InitTest()
        {
            All.Add(new Test { Name = "Fon", Default = 30, Values = new() { 30, 100, 250 }, IsActive = true });
            All.Add(new Test { Name = "Button", Default = 1, Values = new() { 0, 1 }, IsActive = true });

            Active = All.Where(x => x.IsActive).ToList();
        }

        public Dictionary<string, int> GetNewAB()
        {
            var dict = new Dictionary<string, int>();

            foreach (var test in Active)
            {
                dict[test.Name] = test.GetValue(_strategy);
            }

            return dict;
        }
    }
}