using System.Linq;
using UnityEngine;

namespace Helpers
{
    public static class StringUtils
    {
        /// <summary>
        /// Converts a KeyCode to a string of numbers or letters.
        /// </summary>
        public static string KeyCodeToNumbersOrLetters(KeyCode keycode)
        {
            string input = keycode.ToString();
            string numbers = new(input.Where(char.IsDigit).ToArray());
            string letters = new(input.Where(char.IsLetter).ToArray());

            if (!string.IsNullOrEmpty(numbers))
            {
                return numbers;
            }

            return string.IsNullOrEmpty(letters) ? "?" : letters;
        }
    }
}