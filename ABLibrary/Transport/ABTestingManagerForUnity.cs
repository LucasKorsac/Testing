using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ABLibrary.Transport
{
    public class ABTestingManagerForUnity : IABManager
    {
        private ABManager _manager;

        private bool _isInitialized;

        private string _currentGameId;

        public bool IsInitialized => _isInitialized;

        public string CurrentGameId => _currentGameId;

        //public async Task InitAsync(string gameId)
        //{
        //    try
        //    {
        //        // URL ASP.NET API
        //        var transport =
        //            new HttpABTransport(
        //                "https://localhost:5001/");

        //        // локальное хранилище
        //        string storagePath =
        //            GetPersistentDataPath();

        //        var storage =
        //            new FileStorageForUnity(storagePath);

        //        // клиент SDK
        //        var client =
        //            new ABClient(
        //                transport,
        //                storage);

        //        // менеджер
        //        _manager =
        //            new ABManager(client);

        //        // инициализация
        //        await _manager.InitAsync(gameId);

        //        _currentGameId = gameId;

        //        _isInitialized = true;

        //        Console.WriteLine(
        //            $"AB initialized for game: {gameId}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(
        //            $"Initialization failed: {ex.Message}");

        //        _isInitialized = false;
        //    }
        //}

        public async Task InitAsync(string gameId, string instanceId = "")
        {
            try
            {
                var transport = new HttpABTransport("http://192.168.100.6:5001/");
                string storagePath = GetPersistentDataPath();
                var storage = new FileStorageForUnity(storagePath);
                var client = new ABClient(transport, storage);
                _manager = new ABManager(client);

                await _manager.InitAsync(gameId, instanceId);

                _currentGameId = gameId;
                _isInitialized = true;

                Console.WriteLine($"AB initialized for game: {gameId}, instance: {instanceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization failed: {ex.Message}");
                _isInitialized = false;
            }
        }

        public string GetVariant(string testName)
        {
            if (!_isInitialized || _manager == null)
                return "default";

            return _manager.GetVariant(testName);
        }

        public async Task TrackAsync(
            string testName,
            string userId,
            string eventType = "conversion")
        {
            if (!_isInitialized || _manager == null)
                return;

            await _manager.TrackAsync(
                testName,
                userId,
                eventType);
        }

        public async Task FlushAsync()
        {
            if (!_isInitialized || _manager == null)
                return;

            await _manager.FlushAsync();
        }

        private string GetPersistentDataPath()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return "/data/data/com.yourcompany.yourapp/files";
#elif UNITY_IOS && !UNITY_EDITOR
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "ZeroErrorGame");
#endif
        }
    }
}