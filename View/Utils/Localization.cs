using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace View.Utils
{
    public class ModelLocalizaton
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("en")]
        public string En { get; set; }
    }

    public static class Localization
    {
        private static Dictionary<string, ModelLocalizaton>? localization;
        private static string lang = "en";

        public static void Load(string path, string defaultLang = "id")
        {
            string json = File.ReadAllText(path);
            localization = JsonSerializer.Deserialize<Dictionary<string, ModelLocalizaton>>(json);
            lang = defaultLang;
        }

        public static void SetDefaultLang(string language)
        {
            lang = language;
        }

        public static string GetLangKey(string key)
        {
            if (localization == null)
            {
                return "[[Localization not loaded]]";
            }

            if (!localization.ContainsKey(key))
            {
                return $"[[{key}]]";
            }

            return lang == "en" ? localization[key].En : localization[key].Id;
        }
    }
}
