using TMPro;
using UnityEngine;

namespace ErisJGDK.Base.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class VersionText : MonoBehaviour
    {
        [SerializeField] private App _app;

        [Space(5f)]

        [SerializeField] private string _prefix = "v";
        [SerializeField] private string _suffix;

        private void Awake()
        {
            string version = _app != null ? _app.Version : Application.version;
            GetComponent<TMP_Text>().text = _prefix + version + _suffix;
        }
    }
}