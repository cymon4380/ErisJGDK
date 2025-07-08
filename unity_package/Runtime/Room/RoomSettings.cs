using System;
using UnityEngine;

namespace ErisJGDK.Base
{
    public class RoomSettings
    {
        public static bool AudienceEnabled;
        public static bool HideCode;
        public static bool SkipTutorial;
        public static string Password;
        public static string ModerationPassword;

        public static void Save()
        {
            PlayerPrefs.SetInt("AudienceEnabled", Convert.ToInt32(AudienceEnabled));
            PlayerPrefs.SetInt("HideCode", Convert.ToInt32(HideCode));
            PlayerPrefs.SetInt("SkipTutorial", Convert.ToInt32(SkipTutorial));
            PlayerPrefs.SetString("Password", Password);
            PlayerPrefs.SetString("ModerationPassword", ModerationPassword);
        }

        public static void Load()
        {
            AudienceEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("AudienceEnabled"));
            HideCode = Convert.ToBoolean(PlayerPrefs.GetInt("HideCode"));
            SkipTutorial = Convert.ToBoolean(PlayerPrefs.GetInt("SkipTutorial"));
            Password = PlayerPrefs.GetString("Password");
            ModerationPassword = PlayerPrefs.GetString("ModerationPassword");
        }
    }
}