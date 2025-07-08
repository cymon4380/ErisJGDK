using ErisJGDK.Base.Miscellaneous;
using System.Globalization;
using UnityEngine;

namespace ErisJGDK.Base.Utils
{
    public class StringFormat
    {
        public enum RoundMode
        {
            Floor,
            Half,
            Ceil
        }

        public static string SeparateThousands(int number, string separator = " ")
        {
            CultureInfo ciClone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            ciClone.NumberFormat.NumberGroupSeparator = separator;

            return number.ToString("N0", ciClone);
        }

        /// <summary>
        /// Converts seconds to minutes and seconds, e.g. M:SS.
        /// </summary>
        public static string FormatTime(float totalSeconds, TimerFormat format, RoundMode roundMode = RoundMode.Floor)
        {
            float roundedSeconds = RoundWithMode(totalSeconds, roundMode);

            int minutes = Mathf.FloorToInt(roundedSeconds / 60);
            int seconds = Mathf.FloorToInt(roundedSeconds % 60);

            return format switch
            {
                TimerFormat.MSS => $"{minutes}:{LeadingZero(seconds)}",
                TimerFormat.MMSS => $"{LeadingZero(minutes)}:{LeadingZero(seconds)}",
                _ => roundedSeconds.ToString(),
            };
        }

        /// <summary>
        /// Adds a leading zero to a positive one-digit number. Is used in time formatting.
        /// </summary>
        public static string LeadingZero(int number)
        {
            return number >= 0 && number < 10 ? "0" + number : number.ToString();
        }

        public static float RoundWithMode(float number, RoundMode roundMode)
        {
            return roundMode switch
            {
                RoundMode.Ceil => Mathf.Ceil(number),
                RoundMode.Half => Mathf.Round(number),
                _ => Mathf.Floor(number)
            };
        }
    }
}
