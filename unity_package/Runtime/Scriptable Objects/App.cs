using System.Linq;
using UnityEngine;

namespace ErisJGDK.Base
{
    [CreateAssetMenu(fileName = "App", menuName = "App")]
    public class App : ScriptableObject
    {
        public enum AppType
        {
            Writing,
            Drawing,
            Trivia,
            Miscellaneous
        }

        [Header("General")]

        [SerializeField] private string _name;
        [SerializeField] private string _tag;
        [SerializeField] private string _version;

        [Space(8)]
        [Min(1)]
        [SerializeField] private int _minPlayers = 1;

        [Min(1)]
        [SerializeField] private int _maxPlayers = 8;

        [Space(8)]
        [SerializeField] private bool _audienceEnabled;
        [SerializeField] private bool _moderationEnabled;

        [Space(8)]
        [SerializeField] private string _sceneName;

        [Header("Menu")]

        [TextArea(3, 6)]
        [SerializeField] private string _description;

        [SerializeField] private AppType _type;
        [SerializeField] private string _estimatedDuration;
        [SerializeField] private bool _hidden;
        [SerializeField] private bool _available = true;
        [SerializeField] private Color _backgroundColor;

        public string Name => _name;
        public string Tag => _tag;
        public string Version => _version;

        public int MinPlayers => _minPlayers;
        public int MaxPlayers => _maxPlayers;
        public bool AudienceEnabled => _audienceEnabled;
        public bool ModerationEnabled => _moderationEnabled;
        public string SceneName => _sceneName;

        public string Description => _description;
        public AppType Type => _type;
        public string EstimatedDuration => _estimatedDuration;
        public bool Hidden => _hidden;
        public bool Available => _available;
        public Color BackgroundColor => _backgroundColor;

        public static App GetApp(string tag)
        {
            return Resources.FindObjectsOfTypeAll<App>().FirstOrDefault(a => a.Tag == tag);
        }
    }
}