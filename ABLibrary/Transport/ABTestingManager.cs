//using ABLibrary.Core;
//using ABLibrary.Storage;
//using ABLibrary.Transport;
//using System.Diagnostics;
//using UnityEngine;
//using static System.Net.Mime.MediaTypeNames;

//public class ABTestingManager : MonoBehaviour
//{
//    public static ABManager Manager;

//    async void Start()
//    {
//        var transport =
//            new HttpABTransport(
//                "http://localhost:5000");

//        var storage =
//            new FileStorage(
//                Application.persistentDataPath);

//        var client =
//            new ABClient(
//                transport,
//                storage);

//        Manager =
//            new ABManager(client);

//        await Manager.InitAsync("my-game");

//        Debug.Log("AB initialized");
//    }
//}