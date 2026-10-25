using System.Threading.Tasks;
using UnityEngine;

namespace ABLibraryUnity.Unity
{
    public sealed class ABUnityLifecycle : MonoBehaviour
    {
        public ABTestingManagerForUnity Client;

        private async void OnApplicationPause(
            bool pauseStatus)
        {
            if (!pauseStatus || Client == null)
                return;

            try
            {
                await Client.FlushAsync();
            }
            catch
            {
                // События должны остаться
                // в локальном буфере SDK.
            }
        }

        private async void OnApplicationQuit()
        {
            if (Client == null)
                return;

            try
            {
                await Client.FlushAsync();
            }
            catch
            {
                // Не теряем события:
                // buffer уже сохранён локально.
            }
        }
    }
}