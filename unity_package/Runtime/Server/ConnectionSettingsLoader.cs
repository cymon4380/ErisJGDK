using UnityEngine;

namespace ErisJGDK.Base.Server
{
    public class ConnectionSettingsLoader : MonoBehaviour
    {
        private void Awake()
        {
            ConnectionSettings.LoadSettings();
        }
    }
}