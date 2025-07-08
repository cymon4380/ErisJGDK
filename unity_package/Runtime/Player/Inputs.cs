using System;
using System.Collections.Generic;

namespace ErisJGDK.Base
{
    public class Inputs
    {
        public enum RecipientType
        {
            ALL_PLAYERS,
            ALL_AUDIENCE,
            ALL,
            CERTAIN
        }

        public string Prompt;
        public List<InputElement> Elements = new();
        public bool SubmitButton;

        public Inputs(string prompt, List<InputElement> elements, bool submitButton)
        {
            Prompt = prompt;
            Elements = elements;
            SubmitButton = submitButton;
        }


        public Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new();

            foreach (InputElement element in Elements)
            {
                string key = element is InputButton ? Guid.NewGuid().ToString() : element.Key;
                data.Add(key, element.Serialize());
            }

            return data;
        }
    }
}