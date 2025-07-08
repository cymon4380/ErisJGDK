using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using ErisJGDK.Base.Extensions;
using System.Linq;

namespace ErisJGDK.Base.Settings
{
    [RequireComponent(typeof(TMP_InputField))]
    public class PasswordField : MonoBehaviour
    {
        private TMP_InputField _inputField;
        private CanvasGroup _canvasGroup;
        private EventSystem _eventSystem;

        private bool _shown;

        public bool Interactable => _canvasGroup != null ? _canvasGroup.interactable && _inputField.interactable : _inputField.interactable;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            _eventSystem = EventSystem.current;

            _inputField.onSelect.AddListener((string _) => ShowPassword());
            _inputField.onDeselect.AddListener((string _) => HidePassword());
        }

        public void ShowPassword()
        {
            _inputField.inputType = TMP_InputField.InputType.Standard;
            _inputField.ForceLabelUpdate();
        }

        public void HidePassword()
        {
            _inputField.inputType = TMP_InputField.InputType.Password;
            _inputField.ForceLabelUpdate();
        }

        public void CopyPassword()
        {
            if (string.IsNullOrWhiteSpace(_inputField.text))
                return;

            GUIUtility.systemCopyBuffer = _inputField.text;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _eventSystem.GetHoveredUIElements().Contains(gameObject) && !Interactable && !_shown)
            {
                _shown = true;
                ShowPassword();
            }
            else if (Input.GetMouseButtonUp(0) && _shown)
            {
                _shown = false;
                HidePassword();
            }

            if (Input.GetMouseButton(1) && _eventSystem.GetHoveredUIElements().Contains(gameObject))
                CopyPassword();
        }
    }
}