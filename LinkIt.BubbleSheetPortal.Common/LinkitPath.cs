using System.Collections.Generic;
using System.IO;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class LinkitPath
    {
        private static readonly List<string> InvalidStartCharacters = new List<string> { "&", "#", "!", "$", "%", "^", "*" };

        public static string GetFileName(string path)
        {
            var fileName = Path.GetFileName(path);
            fileName = RemoveInvalidCharacters(fileName);
            if (string.IsNullOrWhiteSpace(fileName)) fileName = "_";
            //fileName = System.Web.HttpUtility.UrlEncode(fileName);

            return fileName;
        }

        private static string RemoveInvalidCharacters(string name)
        {
            if (name == null) return null;

            foreach (var invalidStartCharacter in InvalidStartCharacters)
            {
                name = name.Replace(invalidStartCharacter, string.Empty);
            }

            return name;
        }

        private static string RemoveInvalidStartCharacters(string name)
        {
            if (name == null) return null;
            name = name.Trim();
            if (StartWithInvalidCharacter(name))
            {
                name = name.Remove(0, 1);
                name = RemoveInvalidStartCharacters(name);
            }

            return name;
        }

        private static bool StartWithInvalidCharacter(string name)
        {
            if (name == null) return false;

            foreach(var invalidStartCharacter in InvalidStartCharacters)
            {
                if (name.StartsWith(invalidStartCharacter)) return true;
            }

            return false;
        }
    }
}
