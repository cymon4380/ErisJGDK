using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace ErisJGDK.Base
{
    public static class ConnectionSettings
    {
        public const string SettingsFile = "connectionSettings.txt";

        public static string Host = "127.0.0.1";
        public static int Port = 2096;
        public static string WebSocketSuffix = "";
        public static string Url = "/createRoom";
        public static bool IsSecuredConnection = false;
        public static string JoinUrl = "127.0.0.1:4267";

        public static string GetString(bool http)
        {
            string protocol;

            if (http)
                protocol = IsSecuredConnection ? "https" : "http";
            else
                protocol = IsSecuredConnection ? "wss" : "ws";

            return string.Concat(new object[]
            {
                protocol, "://",
                Host, ":", Port,
                http ? string.Empty : WebSocketSuffix,
                Url
            });
        }

        public static void LoadSettings()
        {
            string path = Path.Combine(Application.dataPath, SettingsFile);
            if (!File.Exists(path))
            {
                Debug.LogWarning("Settings file cannot be found.");
                return;
            }

            var settings = SettingsParser.GetSettings(path);

            foreach (FieldInfo fieldInfo in typeof(ConnectionSettings).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                try
                {
                    if (!settings.ContainsKey(fieldInfo.Name))
                        continue;

                    object value = Convert.ChangeType(settings[fieldInfo.Name], fieldInfo.FieldType);
                    fieldInfo.SetValue(null, value);

                    Debug.Log($"Set connection settings value: {fieldInfo.Name} = {value}");
                }
                catch(Exception ex)
                {
                    Debug.LogError($"Failed to set connection settings value \"{fieldInfo.Name}\": {ex.Message}");
                }
            }
        }
    }

    public static class SettingsParser
    {
        public static Dictionary<string, string> GetSettings(string filePath)
        {
            Dictionary<string, string> settings = new();

            using StreamReader reader = new(filePath);
            string content = reader.ReadToEnd();

            string[] lines = content.Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.StartsWith('#'))
                    continue;

                string[] splitted = line.Split('=');
                string key = splitted[0];
                string value = line[(key.Length + 1)..].Trim();

                settings[key.Trim()] = value;
            }

            return settings;
        }
    }

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
