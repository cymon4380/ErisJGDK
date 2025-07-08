using System;
using System.Collections.Generic;
using System.Drawing;

namespace ErisJGDK.Base
{
    public abstract class InputElement
    {
        public enum InputType
        {
            Text,
            Checkbox,
            Range,
            Button,
            Label,
            Image
        }

        public string Key { get; private set; }
        public InputType Type { get; private set; }
        public string HtmlClass { get; private set; }

        public InputElement(string key, InputType inputType, string htmlClass = null)
        {
            Key = key;
            Type = inputType;
            HtmlClass = htmlClass;
        }

        /// <summary>
        /// Serializes the element to send it to players as JSON.
        /// </summary>
        public abstract Dictionary<string, object> Serialize();
    }

    public class InputText : InputElement
    {
        public int MaxLength { get; private set; } = 100;
        public bool Required { get; private set; }

        public InputText(string key, int maxLength = 100, bool required = true) : base(key, InputType.Text)
        {
            MaxLength = maxLength;
            Required = required;
        }

        public override Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "type", Type.ToString().ToLower() },
                { "maxLength", MaxLength },
                { "required", Required },
            };

            return data;
        }
    }

    public class InputCheckbox : InputElement
    {
        public string Label { get; private set; }

        public InputCheckbox(string key, string label) : base(key, InputType.Checkbox)
        {
            Label = label;
        }

        public override Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "type", Type.ToString().ToLower() },
                { "label", Label },
            };

            return data;
        }
    }

    public class InputRange : InputElement
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public int Step { get; private set; }
        public bool ShowValue { get; private set; }
        public int? Value { get; private set; }

        public InputRange(
                string key,
                int min, int max,
                int step,
                bool showValue = true,
                int? value = null,
                string htmlClass = null
            ) : base(key, InputType.Range, htmlClass)
        {
            Min = min;
            Max = max;
            Step = step;
            ShowValue = showValue;
            Value = value;
        }

        public override Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "type", Type.ToString().ToLower() },
                { "showValue", ShowValue },
                { "min", Min },
                { "max", Max },
                { "step", Step },
                { "value", Value },
            };

            return data;
        }
    }

    public class InputButton : InputElement
    {
        public string Text { get; private set; }
        public object Value { get; private set; }
        public int? Row { get; private set; }
        public bool Disabled { get; private set; }
        public ButtonImage Image { get; private set; }

        public InputButton(
            string key,
            string text,
            object value,
            int? row = null,
            bool disabled = false,
            string htmlClass = null,
            ButtonImage image = null
        ) : base(key, InputType.Button, htmlClass)
        {
            Text = text;
            Value = value;
            Row = row;
            Disabled = disabled;
            Image = image;
        }

        public override Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "type", Type.ToString().ToLower() },
                { "text", Text },
                { "key", Key },
                { "value", Value },
                { "row", Row },
                { "disabled", Disabled }
            };

            if (Image != null)
            {
                data.Add("image", new Dictionary<string, object>
                {
                    { "src", Image.Source },
                    { "alt", Image.Alt },
                    { "width", Image.Width },
                    { "height", Image.Height }
                });
            }

            return data;
        }
    }

    public class ButtonImage
    {
        public string Source;
        public string Alt;
        public int? Width;
        public int? Height;

        public ButtonImage(string source, string alt = null, int? width = null, int? height = null)
        {
            Source = source;
            Alt = alt;
            Width = width;
            Height = height;
        }
    }

    public class InputLabel : InputElement
    {
        public string Text { get; private set; }
        public Color Color { get; private set; }
        public int Size { get; private set; }

        public InputLabel(
            string text,
            Color color,
            int size = 2,
            string htmlClass = null
        ) : base(Guid.NewGuid().ToString(), InputType.Label, htmlClass)
        {
            Text = text;
            Color = color;
            Size = size;
        }

        public override Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "type", Type.ToString().ToLower() },
                { "text", Text },
                { "size", Size },
                { "color", $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}" },
            };

            return data;
        }
    }

    public class InputImage : InputElement
    {
        public string Source { get; private set; }
        public int? Width { get; private set; }
        public int? Height { get; private set; }
        public bool Draggable { get; private set; }

        public InputImage(
            string source,
            int? width,
            int? height,
            bool draggable,
            string htmlClass = null
        ) : base(Guid.NewGuid().ToString(), InputType.Image, htmlClass)
        {
            Source = source;
            Width = width;
            Height = height;
            Draggable = draggable;
        }

        public override Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "type", Type.ToString().ToLower() },
                { "src", Source },
                { "width", Width },
                { "height", Height },
                { "draggable", Draggable },
            };

            return data;
        }
    }
}