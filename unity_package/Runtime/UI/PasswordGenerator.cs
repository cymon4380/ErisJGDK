using TMPro;
using UnityEngine;

namespace ErisJGDK.Base.UI
{
    public class PasswordGenerator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _passwordField;

        [Min(0)]
        [SerializeField] private int _passwordLength = 5;

        public string Generate()
        {
            int[] digits = new int[_passwordLength];
            for (int i = 0; i < digits.Length; i++)
                digits[i] = Random.Range(0, 10);

            return string.Concat(digits);
        }

        public void GenerateAndApply()
        {
            _passwordField.text = Generate();
        }
    }
}