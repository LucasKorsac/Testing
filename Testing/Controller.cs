using System;
using System.Collections.Generic;
using System.Threading;

namespace Testing
{
    internal class Controller
    {
        private static readonly Lazy<Controller> _instance = new(() => new Controller());

        public static Controller I => _instance.Value;

        private readonly ReaderWriterLockSlim _lock = new();

        private Dictionary<string, int> _currentTests = new();

        public Dictionary<string, int> CurrentTests
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return new Dictionary<string, int>(_currentTests);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public void Init(Dictionary<string, int> ab)
        {
            _lock.EnterWriteLock();
            try
            {
                _currentTests = new Dictionary<string, int>(ab);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public int Get(string name)
        {
            _lock.EnterReadLock();
            try
            {
                return _currentTests.TryGetValue(name, out var val) ? val : 0;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}