using UnityEngine;
using UnityEngine.UI;

namespace ErisJGDK.Base.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenWebsiteButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OpenWebsite);
        }

        public void OpenWebsite()
        {
            string url = string.Concat(new object[]
            {
                ConnectionSettings.IsSecuredConnection ? "https" : "http", "://",
                ConnectionSettings.JoinUrl,
                "?code=", RoomManager.Instance.CurrentRoom.Code
            });

            Application.OpenURL(url);
        }
    }
}