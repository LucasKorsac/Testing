using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Pattern;

namespace Testing
{
    public class ClientTest
    {
        public Tests Tests;
        public TestResults TestResults;
        public Action<Tests>? OnLoadComleted;

        public ClientTest()
        {
            Tests = ;
            // загружаем из файла Tests 
        }

        public void Load()
        {
            // получаем с сервера
            Tests = ;
            // сохраняем в файл Tests 
            OnLoadComleted?.Invoke(Tests);
        }

        public void Save()
        {
            // сохраняем в файл TestResults 
            // отправляем на сервер TestResults
        }
    }

    public class TestResults {
    List<Result>

    }
    public class Tests {
    List<Test>
    }
    public class Result {
        Name+Value
    }
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

        public override string ToString()
        {
            return $"{Name}: {string.Join(",", Values ?? [])} ({Values})";
        }
    }
}
